import {defineStore} from 'pinia';
import type {
    GenericResponse,
    IArticle,
    IArticleResponse,
    IFullArticle,
    IScience,
    IScientificTheory,
    ISearchResponse,
    ISelectedArticle
} from '@/api/type';
import {sendRequest, showErrorMessage} from '@/api/authApi';
import MyLocalStorage from "@/services/myLocalStorage";
import Swal from "sweetalert2";
import router from "@/router";
import {useAuthStore} from "@/stores/authStore";


export type ArticleStoreState = {
    popularArticles: IArticle[] ;
    articles: IArticle[] ;
    isLoading: boolean;
    sciences: IScience[];
    scientificSections:IScientificTheory[];
    sortedOptions:Array<object>;
    filterOptions:Array<object>;
    tagItems:Array<string>;
    cntRec: number;
    totalPage:number;
}

export const useArticleStore = defineStore({
    id: 'article',
    state: (): ArticleStoreState => ({
        popularArticles: [],
        articles:[],
        isLoading:false,
        sciences: [],
        scientificSections:[],
        sortedOptions: [
            {key:0, value:'За релевантністю'},
            {key:10, value:'Спочатку більш нові'},
            {key:11, value:'Спочатку більш старі'},
            {key:12, value:'За кількістю переглядів'}
        ],
        filterOptions: [
            {key:1, value:'Тільки наукові'},
            {key:2, value:'Тільки без DOI'},
        ],
        tagItems:[],
        cntRec:0,
        totalPage:1

    }),
    getters:{},

    actions: {
        //отримати список нових статтей
        async getNewArticleList(pages:number, ):Promise<Array<IArticle>|undefined> {

            const sortParam = 10; //спочатку більш нові
            const  peopleId = MyLocalStorage.getItem('peopleId'); //може повернутися і null
            try{
                this.isLoading=true;
                let newArticles:Array<IArticle>=[];
                const res = await sendRequest<ISearchResponse<IFullArticle<IArticle>>>(
                    'GET',
                    'MenuSearching/SearchWithFilters',
                    {
                        'Pages': pages,
                        'TypeOrder':sortParam,
                        'idPeopleForSelect':peopleId
                    }

                );
                console.log("new articles", res);
                res?.articles?.map((item)=>{
                    let article = this.parseArticleAndReaction(item);
                    if (article!==undefined){
                        // console.log(article);
                        newArticles.push(article);
                    }
                })
                console.log("New_articles []= ", newArticles);
                return newArticles;
            }catch (error) {
               console.error(error);
            }finally {
                this.isLoading =false;
            }

        },
        //отримати список популярних статтей
        async getPopularArticleList(pages:number) {
            try{
                this.isLoading=true;
                this.popularArticles=[];
                const res = await this.getPopularArticle(pages);
                res?.articles?.map((item)=>{
                    let article = this.parseArticleAndReaction(item);
                    if (article!==undefined){
                        // console.log(article);
                        this.popularArticles.push(article);
                    }
                })
                console.log('allPages=',res.allPages)
                this.totalPage = res.allPages;
                //console.log("PopularArticle[]=", this.articles);
            }catch (error) {
                console.error(error);
            }finally {
                this.isLoading =false;
            }
        },

        //отримати список популярних статтей, що відповідає наступній сторінці
        //для реалізації нескінченної стрічки
        //статті, які ми будемо отримувати уцьому запиті будемо дописувати в кінець масиву
        //сам масив не обнуляємо
        async getMorePopularArticle(pages:number) {
            try{
                this.isLoading=true;
                const res = await this.getPopularArticle(pages);
                res?.articles?.map((item)=>{
                    let article = this.parseArticleAndReaction(item);
                    if (article!==undefined){
                        // console.log(article);
                        this.popularArticles.push(article);
                    }
                })
                console.log("PopularArticle[]=", this.popularArticles);
            }catch (error) {
                console.error(error);
            }finally {
                this.isLoading =false;
            }
        },
        //отримати список моїх статей
         async getMySelectedArticleList(peopleId:string, page:number) {
            try{
                this.cntRec=0;
                this.articles=[];
                this.isLoading=true;
                const res = await sendRequest<ISearchResponse<IFullArticle<ISelectedArticle>>>('GET',
                    'selected_articles/getSelectedArticle',
                    {
                        idPeople: peopleId,
                        page:page
                    }
                );
                //console.log("mySelectedArticles=", res);
                this.articles=[];
                res?.articles?.map(item=>{
                    //перетворюємо IFullArticle<ISelectedArticle> в IFullArticle<IArticle>
                    let newItem:IFullArticle<IArticle> = {
                        articles:item.articles.article_,
                        selected:item.selected,
                        emotion:item.emotion,
                        countReactions:item.countReactions
                    }
                    //console.log('newItem=',newItem)
                    let article = this.parseArticleAndReaction(newItem);
                    if (article!==undefined){
                        this.articles.push(article);
                    }
                })

                //console.log("mySelectedArticles[]=", this.articles);
                this.cntRec=this.articles?.length;
                if(res!==undefined && res.allPages!==undefined)
                   this.totalPage=res.allPages;
                else this.totalPage=1;
            }catch (error:any) {
                console.error(error);
                this.totalPage=1;
                this.cntRec=0;
                if('name' in error
                    && error.name==='AxiosError'
                    && error.response?.status===401){
                    await Swal.fire({
                        icon: 'error',
                        title: 'Помилка авторизації',
                        text: showErrorMessage(error)
                    });
                    useAuthStore().delUserData();
                    router.push('/login');
                }
            }finally {
                this.isLoading =false;
            }

        },
        async getScienceSectionArticleList(scienceId:string) {
            try{
                this.cntRec=0;
                this.articles=[];
                this.isLoading=true;
                const res = await sendRequest<Array<IArticle>>('GET',
                    '/api/SearchOnClick/ForScientificArticle',
                    {theory_id: scienceId,
                             amount:8}
                );
                console.log("ScienceSectionArticleList=", res);
                /*this.transformArticleAndReactionToListArticle( res);*/
                this.articles = res;
                this.articles?.forEach(item=>{
                    if (item.tag!==undefined && item.tag!=null)
                        item.tagItems = item.tag.split(',').filter(Boolean);

                });
                console.log("ScienceSectionArticleList[]=", this.articles.values());
                this.cntRec=this.articles.length;
            }catch (error) {
                console.error(error);
            }finally {
                this.isLoading =false;
            }

        },
        async getArticle(articleId:string, peopleId:string):Promise<IArticle|undefined> {
            try{
                this.isLoading=true;
                const res = await sendRequest<IFullArticle<IArticle>>(
                    'GET',
                    'Article/GetArticle',
                    {id: articleId,
                            peopleId:peopleId}

                );
                //console.log(res)
                let article =this.parseArticleAndReaction(res);
                console.log("article=", article);
                return article;
            }catch (error) {
               // console.error(error);
                await Swal.fire({
                    icon: 'error',
                    title: 'Опс... Щось пішло не так!',
                    text: showErrorMessage(error)
                });
                throw error
            }finally {
                this.isLoading =false;
            }

        },
        //збільшити кількість переглядів статті при закритті сторінки з нею
        async incArticleView(articleId:string){
            try{
                 await sendRequest<void>(
                    'GET',
                    'article/AddView',
                    {articleId: articleId}

                );
            }catch (error) {
                console.error(error);
            }

        },
        //додати статтю до Обране
        async addToFavorites(ArticleId:string){
            let peopleId = MyLocalStorage.getItem('peopleId');
            let data = new FormData();
            data.set('ArticleId',ArticleId);
            data.set('PeopleId', peopleId);
            try{
                const res = await sendRequest<GenericResponse>(
                    'POST',
                    'selected_articles/addSelectArticle',
                    undefined,
                    data
                );
                let article =this.articles.find(item=>item.id===ArticleId)
                if(article?.selected!==undefined) article.selected=true;
                Swal.fire({
                    icon: 'info',
                    title: res ? res : 'Виконано',
                    text: ''
                });
                //console.log(res);
            }catch(error){
                throw error
            }
        },
        //вилучити статтю з обраного
        async deleteFromFavorites(ArticleId:string){
            try{
                const res= await this.deleteUserFavorites(ArticleId)
                let article =this.articles.find(item=>item.id===ArticleId)
                if(article?.selected!==undefined) article.selected=false;
                //console.log(res);
                Swal.fire({
                    icon: 'info',
                    title: res,
                    text: ''
                });
            }catch(error){
                throw error;
            }

        },
        /*при вилученні статті з обраних на сторінці збережені, потрібно візуально вилучати статтю,
        * а не тільки змінювати значок закладки*/
        async deleteFromSelectedArticle(ArticleId:string){
            try{
                const res= await this.deleteUserFavorites(ArticleId);
                console.log(res);
                Swal.fire({
                    icon: 'info',
                    title: res,
                    text: ''
                });
                this.articles = this.articles.filter(item=>item.id!==ArticleId)
            }catch(error:any){
                console.log(error)
                Swal.fire({
                    icon: 'error',
                    title: 'Не вдалося вилучити статтю з обраного',
                    text: showErrorMessage(error)
                });
                if('name' in error
                    && error.name==='AxiosError'
                    && error.response?.status===401){
                  useAuthStore().delUserData();
                  router.push('/login');
                }

                //throw error;
            }
        },
        //отримати перелік наукових сфер
        async getScienceList() {
            try{
                const res = await sendRequest<Array<IScience>>(
                    'GET',
                    'sciences/getSiences');
                    this.sciences=res;
                    this.sciences?.map((item)=>{
                        if (!this.tagItems.includes(item.name))
                            this.tagItems.push(item.name)
                    });

            }catch (error) {
                console.error(error);
            }

        },
        //отримати розділи наукових сфер
        async getScienceSectionList() {
            try{
                const res = await sendRequest<Array<IScientificTheory>>(
                    'GET',
                    'scientific_theories/getscientific_theories'
                );
                this.scientificSections=res;
                //console.log("scientificSections=", res);
            }catch (error) {
                console.error(error);
            }
        },
        async getScienceTheoryByScienceId(scienceId:string) {
            try{
                const  peopleIdForSelected = MyLocalStorage.getItem('peopleId'); //може повернутися і null
                const res = await sendRequest<ISearchResponse<IFullArticle<IArticle>>>(
                    'GET',
                    'MenuSearching/SearchWithFilters',
                    {
                        'Pages': 0,
                        'scienceId':scienceId,
                        'idPeopleForSelect':peopleIdForSelected
                    }

                );
                if(res?.theories!==undefined)
                  this.scientificSections=res.theories;
                console.log("scientificSections=", res);
            }catch (error) {
                console.error(error);
            }
        },
        async saveArticle(newArticle:IArticle):Promise<IArticleResponse|undefined> {
            try{
                this.isLoading=true;
                const res = await sendRequest<IArticleResponse>(
                    'POST',
                    'article/CreateArticle',
                    undefined,
                    newArticle
                );
               // console.log("saveArticles =", res);
                /*this.articles=[];
                this.transformArticleAndReactionToListArticle(res.articles);
                this.cntRec=this.articles?.length;*/
                this.isLoading=false;
                return res;

            }catch (error) {
                this.isLoading=false;
               // console.log('error from SaveArticle:', error);
                throw error;
            }

        },
        //опублікувати статтю (при натисканні на кнопку на картці статті)

        async updateArticle(article:IArticle):Promise<string> {
            try{
                this.isLoading=true;
                const res = await sendRequest<string>(
                    'POST',
                    'article/RedactArticle',
                    undefined,
                    article
                );
                //console.log("updateArticles:", res);
                this.isLoading =false;
                return res
            }catch (error) {
                //console.error(error);
                this.isLoading =false;
                throw error;
            }

        },
        async deleteArticle(delArticle:IArticle) {
            try{
                this.isLoading=true;
                const res = await sendRequest<string>(
                    'POST',
                    'article/deleteArticle',
                    undefined,
                    delArticle
                );
                this.articles = this.articles.filter(item=>item.id!=delArticle.id);
                console.log("delArticle = ", res);
                this.isLoading =false;
                Swal.fire({
                    icon: 'info',
                    title: res,
                    text: ''
                });
            }catch (error:any) {
                //console.error(error);
                this.isLoading =false;
                await Swal.fire({
                    icon: 'error',
                    title: 'Помилка при видаленні статті',
                    text: showErrorMessage(error)
                });
                if('name' in error
                    && error.name==='AxiosError'
                    && error.response?.status===401){
                    useAuthStore().delUserData();
                    router.push('/login');
                }
            }

        },
        async searchArticlesByParam(pages:number,
                                    searchStr?:string,
                                    year?:number|null,
                                    filters?:number|null,
                                    typeOrder?:number|null,
                                    tags?:string|null,
                                    peopleId?:string|null,
                                    scienceId?:string|null,
                                    scienceTheoryId?:string|null,
                                    ){
            try{
                this.cntRec=0;
                this.isLoading=true;
                this.totalPage=0;
                const  peopleIdForSelected = MyLocalStorage.getItem('peopleId'); //може повернутися і null
                const res = await sendRequest<ISearchResponse<IFullArticle<IArticle>>>(
                    'GET',
                    'MenuSearching/SearchWithFilters',
                    {
                        'Pages': pages,
                        'SearchString': searchStr,
                        'year': year,
                        'Filters': filters,
                        'TypeOrder':typeOrder,
                        'tags':tags,
                        'peopleId':peopleId,
                        'scienceId':scienceId,
                        'theoryId':scienceTheoryId,
                        'idPeopleForSelect':peopleIdForSelected
                    }

                );
                console.log("search articles", res);
                this.articles = [];
                this.transformArticleAndReactionToListArticle( res?.articles);
                console.log("articles []= ", this.articles);

                this.cntRec = this.articles?.length;
                this.totalPage = res.allPages;
            }catch (error) {
                this.cntRec=0;
                this.articles=[];
                this.totalPage=1;
                console.error(error);
            }finally {
                this.isLoading =false;
            }

        },
        async saveFile(data:FormData):Promise<string> {
            try{
                this.isLoading=true;
                const res = await sendRequest<string>(
                    'POST',
                    '/files/addFiles',
                    undefined,
                    data
                );
                //console.log("saveFile:", res);
                return res;
            }catch (error) {
                console.error(error);
                throw error;
            }finally {
                this.isLoading =false;
            }
        },
        async downloadFiles(article_id:string) {

          // варіант при відсутності авторизації
            const link = document.createElement('a');
            link.href = ' http://127.0.0.11:5000/api/Files/GetArchivWithFiles?id='+article_id;
            console.log(link)
            document.body.appendChild(link);
            link.click();
            // Видалення посилання після завершення завантаження
            document.body.removeChild(link);

            /*try {
                const response = await sendRequest<Blob>(
                    'GET',
                    'Files/GetArchivWithFiles',
                    {id:article_id}
                );
                // Створюємо посилання на отриманий архів
                const blob = new Blob([response], { type: 'application/zip' });
                const arhiv = this.blobToFile(blob,'Archiv.zip')
                const url = window.URL.createObjectURL(arhiv);
                // Створюємо посилання для завантаження архіву
                const link = document.createElement('a');
                link.href = url;
                //link.setAttribute('download', );
                document.body.appendChild(link);
                link.click();

                // Видалення посилання після завершення завантаження
                document.body.removeChild(link);
            }catch (error) {
                console.error('Помилка під час завантаження файлів:', error);

            }*/
        },
        blobToFile (theBlob: Blob, fileName:string): File {
            const b: any = theBlob;
            //A Blob() is almost a File() - it's just missing the two properties below which we will add
            b.lastModifiedDate = new Date();
            b.name = fileName;
            return theBlob as File;
        },
        parseArticleAndReaction(item:IFullArticle<IArticle>):IArticle|undefined{
           let article = item?.articles;
            //console.log("article=", article);
            if (article!==undefined){
                if(item.articles?.tag!==undefined && item.articles?.tag!==null)
                   article.tagItems = item.articles.tag.split(',').filter(Boolean);
                article.reaction = item.emotion;
                article.countLike = item.countReactions;
                article.selected = item.selected;
                return article;
            }
        },
        transformArticleAndReactionToListArticle(data:Array<IFullArticle<IArticle>>){
            data?.map((item)=>{
                let article = this.parseArticleAndReaction(item);
                if (article!==undefined){
                   // console.log(article);
                    this.articles.push(article);
                }
            })
        },
        //отримати список популярних статтей
        async getPopularArticle(pages:number):Promise<ISearchResponse<IFullArticle<IArticle>>> {

            const sortParam = 12; //за кількістю переглядів
            const  peopleId = MyLocalStorage.getItem('peopleId'); //може повернутися і null
            const res = await sendRequest<ISearchResponse<IFullArticle<IArticle>>>(
                    'GET',
                    'MenuSearching/SearchWithFilters',
                    {
                        'Pages': pages,
                        'TypeOrder': sortParam,
                        'idPeopleForSelect': peopleId
                    });
            console.log("popular articles", res);
            return res;
        },
        async deleteUserFavorites(ArticleId:string) {
            let peopleId = MyLocalStorage.getItem('peopleId');
            let data = new FormData();
            data.set('articleId', ArticleId);
            data.set('peopleId', peopleId);
            return await sendRequest<string>(
                'POST',
                'selected_articles/deleteSelectArticle',
                undefined,
                data
            );


        }
    }
});

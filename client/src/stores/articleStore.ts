import { defineStore } from 'pinia';
import type {
    ArticleResponse,
    GenericResponse,
    IArticle, IArticleAndReactions,
    IPeople,
    IScience,
    IScientificTheory,
    ISearchResponse,
    ISelectedArticle
} from '@/api/type';
import { sendRequest} from '@/api/authApi';
import MyLocalStorage from "@/services/myLocalStorage";

export type ArticleStoreState = {
    newArticles: IArticle[] ;
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
        newArticles: [],
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
        async getNewArticleList() {
            try {
                this.isLoading=true;
                const res = await sendRequest<Array<IArticle>>(
                    'GET',
                    'mainTabArticles/newArticle',
                    {amount:4}
                );
                console.log("NewArticle=", res);
                console.log("NewArticle[]=", this.newArticles)
                this.newArticles=res;
                this.newArticles?.forEach(item=>{
                    item.tagItems = item.tag?.split(',').filter(Boolean);

                }) ;
            }catch(error){
                this.newArticles=[];
                console.error(error);
            }finally{
              this.isLoading=false;
            }

        },
        //отримати список популярних статтей
        async getPopularArticleList(amount:number) {
            try{
                this.isLoading=true;
                const res = await sendRequest<Array<IArticle>>(
                    'GET',
                    'mainTabArticles/popularArticle',
                    {amount:4}
                );
                this.articles =res;
                this.articles.forEach(item=>{
                    item.tagItems = item.tag.split(',').filter(Boolean);

                });
                console.log("PopularArticle[]=", this.articles);
                console.log("PopularArticle=", res);
            }catch (error){
                console.error(error);
                this.articles = [];
                this.cntRec=0;
            }finally {
                this.isLoading =false;
            }
        },
        //отримати список моїх статей
        async getMyArticleList(peopleId:string) {
            try{
                this.isLoading=true;
                const res = await sendRequest<Array<IArticleAndReactions>>('GET',
                    'article/GetArticlesForUser',
                    {id_people: peopleId}
                );
                console.log("myArticles=", res);
                this.articles=[];
                res?.forEach((item)=>{
                    let article = item?.articles;
                    console.log("article=", article);
                    if (article!==undefined){
                        article.tagItems = item.articles?.tag?.split(',').filter(Boolean);
                        article.reaction = item.emotion;
                        article.countLike = item.countReactions;
                        this.articles.push(article);
                    }
                })
                /*this.articles?.forEach(item=>{
                    item.tagItems = item.tag?.split(',').filter(Boolean);

                });*/
                console.log("myArticles[]=", this.articles.values());
                this.cntRec=this.articles.length;
            }catch (error) {
                console.error(error);
                this.articles = [];
                this.cntRec=0;

            }finally {
                this.isLoading =false;
            }

        },
        async getArticle(articleId:string):Promise<IArticle|undefined> {
            try{
                this.isLoading=true;
                const res = await sendRequest<IArticleAndReactions>(
                    'GET',
                    'Article/GetArticle',
                    {id: articleId}
                );
                let article = res?.articles;
                if(article!==undefined){
                    article.tagItems = res?.articles?.tag?.split(',').filter(Boolean);
                    article.reaction = res?.emotion;
                    article.countLike = res?.countReactions;
                }
                //console.log("article=", article);
                return article;
            }catch (error) {
                console.error(error);
            }finally {
                this.isLoading =false;
            }

        },
        //додати статтю до Обране
        async addToFavorites(ArticleId:string){
            let userId = MyLocalStorage.getItem('userId');
            sendRequest<GenericResponse>(
                'POST',
                'add_favorites_article',
                undefined,
                {'ArticleId':ArticleId, 'UserId':userId}) //уточнить адресу
                .then((res)=>{
                    console.log(res);
                }, (error) =>{
                    console.log(error)
                });
        },
        //отримати перелік наукових сфер
        async getScienceList() {
            try{
                this.isLoading=true;
                const res = await sendRequest<Array<IScience>>(
                    'GET',
                    'sciences/getSiences');
                    this.sciences=res;
                    this.sciences?.forEach((item)=>{
                        if (!this.tagItems.includes(item.name))
                            this.tagItems.push(item.name)
                    });

            }catch (error) {
                console.error(error);
            }finally {
                this.isLoading =false;
            }

        },
        //отримати розділи наукових сфер
        async getScienceSectionList() {
            try{
                this.isLoading=true;
                const res = await sendRequest<Array<IScientificTheory>>(
                    'GET',
                    'scientific_theories/getscientific_theories'
                );
                this.scientificSections=res;
                //console.log("scientificSections=", res);
            }catch (error) {
                console.error(error);
            }finally {
                this.isLoading =false;
            }
        },
        async saveArticle(newArticle:IArticle) {
            try{
                this.isLoading=true;
                const res = await sendRequest<ArticleResponse>(
                    'POST',
                    'article/CreateArticle',
                    undefined,
                    newArticle
                );
                this.articles = res.Articles;
                this.articles?.forEach(item=>{
                    item.tagItems = item.tag?.split(',').filter(Boolean);

                });
                this.cntRec=this.articles.length;
                //console.log("saveArticles =", res);
            }catch (error) {
                this.articles=[];
                this.cntRec=0;
                console.error(error);
            }finally {
                this.isLoading =false;
            }

        },
        async updateArticle(article:IArticle) {
            try{
                this.isLoading=true;
                const res = await sendRequest<GenericResponse>(
                    'POST',
                    'article/RedactArticle',
                    undefined,
                    article
                );
                console.log("updateArticles:", res);
            }catch (error) {
                console.error(error);
            }finally {
                this.isLoading =false;
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
            }catch (error) {
                console.error(error);
            }finally {
                this.isLoading =false;
            }

        },
        async searchArticlesByParam(searchStr:string, pages:number, year:number|null,filters:number|null,typeOrder:number|null){
            try{
                this.isLoading=true;
                const res = await sendRequest<ISearchResponse>(
                    'GET',
                    'MenuSearching/SearchWithFilters',
                    {
                        'SearchString': searchStr,
                        'Pages': pages,
                        'Filters': filters,
                        'year': year,
                        'TypeOrder':typeOrder
                    }

                );
                //console.log("search articles", res);
                this.articles = res.articles;
                //console.log("articles []= ", this.articles);
                this.articles?.forEach(item=>{
                    item.tagItems = item.tag?.split(',').filter(Boolean);

                })
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
        async saveFile(data:FormData) {
            try{
                this.isLoading=true;
                const res = await sendRequest<GenericResponse>(
                    'POST',
                    '/files/addFiles',
                    undefined,
                    data
                );
                console.log("saveFile:", res);
            }catch (error) {
                console.error(error);
            }finally {
                this.isLoading =false;
            }
        },
        async downloadFiles(article_id:string) {

          /* варіант при відсутності авторизації
            const link = document.createElement('a');
            link.href = ' http://127.0.0.11:5000/api/Files/GetArchivWithFiles?id='+article_id;
            console.log(link)
            document.body.appendChild(link);
            link.click();
            // Видалення посилання після завершення завантаження
            document.body.removeChild(link);
           */
            try {
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

            }
        },
        blobToFile (theBlob: Blob, fileName:string): File {
            const b: any = theBlob;
            //A Blob() is almost a File() - it's just missing the two properties below which we will add
            b.lastModifiedDate = new Date();
            b.name = fileName;
            return theBlob as File;
        }


    }
});

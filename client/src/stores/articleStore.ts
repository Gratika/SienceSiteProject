import { defineStore } from 'pinia';
import type {GenericResponse, IArticle, IPeople, IScience, IScientificTheory, ISelectedArticle} from '@/api/type';
import { sendRequest} from '@/api/authApi';
import MyLocalStorage from "@/services/myLocalStorage";

export type ArticleStoreState = {
    newArticles: IArticle[] ;
    popularArticles: IArticle[] ;
    myArticles: IArticle[] ;
    isLoading: boolean;
    sciences: IScience[];
    scientificSections:IScientificTheory[];
    searchArticles: IArticle[] ;
    sortedOptions:Array<object>;
    filterOptions:Array<object>;
    tagItems:Array<string>;
    cntRec: number;
}

export const useArticleStore = defineStore({
    id: 'article',
    state: (): ArticleStoreState => ({
        newArticles: [],
        popularArticles: [],
        myArticles:[],
        isLoading:false,
        sciences: [],
        scientificSections:[],
        searchArticles:[],
        sortedOptions: [
            {key:'0', value:'За релевантністю'},
            {key:'1', value:'Спочатку наукові'},
            {key:'2', value:'Спочатку без DOI'},
            {key:'3', value:'За кількістю переглядів'},
            {key:'4', value:'Спочатку більш нові'},
            {key:'5', value:'Спочатку більш старі'}
        ],
        filterOptions: [
            {key:'0', value:'Тільки наукові'},
            {key:'1', value:'Тільки без DOI'},
        ],
        tagItems:[],
        cntRec:0

    }),
    getters:{},

    actions: {
        //отримати список нових статтей
        async getNewArticleList() {
            this.isLoading=true;
            sendRequest<Array<IArticle>>(
                'GET',
                'mainTabArticles/newArticle',
                {amount:4})
                .then((res) =>{
                    this.newArticles=res;
                    this.newArticles?.forEach(item=>{
                        item.tagItems = item.tag?.split('#').filter(Boolean);

                    })
                    this.isLoading =false;
                    console.log("articles", res);
                },(error)=>{
                    this.isLoading =false;
                    console.error(error);
                    //showErrorMessage(error)
                });
        },
        //отримати список популярних статтей
        async getPopularArticleList() {
            this.isLoading=true;
            sendRequest<Array<IArticle>>(
                'GET',
                'mainTabArticles/popularArticle',
                {amount:4})
                .then((res) =>{
                    this.popularArticles=res;
                    this.popularArticles.forEach(item=>{
                        item.tagItems = item.tag.split('#');

                    })
                    this.isLoading =false;
                    console.log("articles", res);
                },(error)=>{
                    this.isLoading =false;
                    console.error(error);
                    //showErrorMessage(error)
                });
        },
        //отримати список моїх статей
        async getMyArticleList(peopleId:string) {
            this.isLoading=true;
            sendRequest<Array<IArticle>>('GET',
                'article/GetArticlesForUser',
                {id_people: peopleId}
            )
                .then((res) =>{
                    this.myArticles=res;
                    this.myArticles?.forEach(item=>{
                        item.tagItems = item.tag?.split('#');

                    });
                    this.isLoading =false;
                    console.log("myArticles", res);
                    this.cntRec=this.myArticles.length;
                },(error)=>{
                    this.isLoading =false;
                    console.error(error);
                    //showErrorMessage(error)
                });
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
            this.isLoading=true;
            sendRequest<Array<IScience>>('GET', 'sciences/getSiences')
                .then((res) =>{
                    this.sciences=res;
                    this.sciences?.forEach((item)=>{
                        if (!this.tagItems.includes(item.name))
                            this.tagItems.push(item.name)
                    });
                    console.log('tag=', this.tagItems)
                    this.isLoading =false;
                    console.log("sciences", res);
                },(error)=>{
                    this.isLoading =false;
                    console.error(error);
                    //showErrorMessage(error)
                });
        },
        //отримати розділи наукових сфер
        async getScienceSectionList() {
            this.isLoading=true;
            sendRequest<Array<IScientificTheory>>('GET', 'scientific_theories/getscientific_theories')
                .then((res) =>{
                    this.scientificSections=res;
                    this.isLoading =false;
                    console.log("scientificSections", res);
                },(error)=>{
                    this.isLoading =false;
                    console.error(error);
                    //showErrorMessage(error)
                });
        },
        async saveArticle(newArticle:IArticle) {
            this.isLoading=true;
            sendRequest<GenericResponse>(
                'POST',
                'article/CreateArticle',
                undefined,
                  newArticle
                )
                .then((res) =>{
                    this.isLoading =false;
                    console.log("articles", res);
                },(error)=>{
                    this.isLoading =false;
                    console.error(error);
                    //showErrorMessage(error)
                });
        },
        async updateArticle(article:IArticle) {
            this.isLoading=true;
            sendRequest<GenericResponse>(
                'POST',
                'article/RedactArticle',
                undefined,
                article
            )
                .then((res) =>{
                    this.isLoading =false;
                    console.log("updateArticles:", res);
                },(error)=>{
                    this.isLoading =false;
                    console.error(error);
                    //showErrorMessage(error)
                });
        },
        async deleteArticle(delArticle:IArticle) {
            this.isLoading=true;
            sendRequest<string>(
                'POST',
                'article/deleteArticle',
                undefined,
                delArticle
            )
                .then((res) =>{
                    this.isLoading =false;
                    this.myArticles = this.myArticles.filter(item=>item.id!=delArticle.id);
                    console.log("delArticle = ", res);
                },(error)=>{
                    this.isLoading =false;
                    console.error(error);
                    //showErrorMessage(error)
                });
        },
        async searchArticlesByParam(searchStr:string) {
            this.isLoading=true;
            sendRequest<Array<IArticle>>(
                'GET',
                'search/search',
                {SearchString: searchStr}

            )
                .then((res) =>{
                    this.isLoading =false;
                    console.log("search articles", res);
                    this.searchArticles = res;
                    this.searchArticles?.forEach(item=>{
                        item.tagItems = item.tag?.split('#');

                    })
                    this.cntRec = this.searchArticles?.length;
                },(error)=>{
                    this.isLoading =false;
                    this.cntRec=0;
                    console.error(error);
                    //showErrorMessage(error)
                },
                );
        },
        async saveFile(data:FormData) {
            this.isLoading=true;
            sendRequest<GenericResponse>(
                'POST',
                '/files/addFiles',
                undefined,
                data
            )
                .then((res) =>{
                    this.isLoading =false;
                    console.log("saveFile:", res);
                },(error)=>{
                    this.isLoading =false;
                    console.error(error);
                    //showErrorMessage(error)
                });
        },
        async downloadFiles(article_id:string, arhiv:string) {
            try {
                const response = await sendRequest<Blob>(
                    'GET',
                    'Files/GetArchivWithFiles',
                    {id:article_id}
                );
                // Створюємо посилання на отриманий архів
                const blob = new Blob([response], { type: 'application/zip' });
                const url = window.URL.createObjectURL(blob);

                // Створюємо посилання для завантаження архіву
                const link = document.createElement('a');
                link.href = url;
                link.setAttribute('download', 'Archiv.zip');
                document.body.appendChild(link);
                link.click();

                // Видалення посилання після завершення завантаження
                document.body.removeChild(link);
            }catch (error) {
                console.error('Помилка під час завантаження файлів:', error);

            }
        }


    }
});

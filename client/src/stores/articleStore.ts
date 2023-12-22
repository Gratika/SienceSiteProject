import { defineStore } from 'pinia';
import type {GenericResponse, IArticle, IPeople, IScience, IScientificTheory, ISelectedArticle} from '@/api/type';
import { sendRequest} from '@/api/authApi';

export type ArticleStoreState = {
    newArticles: IArticle[] ;
    popularArticles: IArticle[] ;
    myArticles: IArticle[] ;
    isLoading: boolean;
    sciences: IScience[];
    scientificSections:IScientificTheory[];
    searchArticles: IArticle[] ;
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
        async addToFavorites(data:ISelectedArticle){
            sendRequest<GenericResponse>(
                'POST',
                'add_favorites_article',
                undefined,
                data) //уточнить адресу
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
                    this.cntRec = this.searchArticles.length;
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
            const filename = arhiv + article_id +'.zip';
            try {
                const response = await sendRequest<ArrayBuffer>(
                    'GET',
                    'Files/GetArchivWithFiles',
                    {id:article_id}
                );
                const blob = new Blob([response], { type: 'application/zip' });
                const url = window.URL.createObjectURL(blob);

                const link = document.createElement('a');
                link.href = url;
                link.download = filename;
                link.click();

                window.URL.revokeObjectURL(url);
            }catch (error) {
                console.error('Помилка під час завантаження файлів:', error);

            }
        }


    }
});

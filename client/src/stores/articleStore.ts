import { defineStore } from 'pinia';
import type {GenericResponse, IArticle, IScience, IScientificTheory, ISelectedArticle} from '@/api/type';
import { sendRequest} from '@/api/authApi';

export type ArticleStoreState = {
    articles: IArticle[] ;
    myArticles: IArticle[] ;
    isLoading: boolean;
    sciences: IScience[];
    scientificSections:IScientificTheory[];
    selectedScienceId: number | null;
}

export const useArticleStore = defineStore({
    id: 'article',
    state: (): ArticleStoreState => ({
        articles: [],
        myArticles:[],
        isLoading:false,
        sciences: [],
        scientificSections:[],
        selectedScienceId: null,
    }),
    getters:{
        //фільтруємо масив з окремими розділами наук
        filteredScientificSections: (state) => {
            return state.scientificSections?.filter((section) => {
                return section.science_id === state.selectedScienceId;
            });
        }
    },

    actions: {
        //отримати список статтей
        async getArticleList() {
            this.isLoading=true;
            sendRequest<Array<IArticle>>('GET', 'article/GetArticles')
                .then((res) =>{
                    this.articles=res;
                    this.isLoading =false;
                    console.log("articles", res);
                },(error)=>{
                    this.isLoading =false;
                    console.error(error);
                    //showErrorMessage(error)
                });
        },
        //отримати список моїх статей
        async getMyArticleList(userId:number) {
            this.isLoading=true;
            sendRequest<Array<IArticle>>('GET', 'article/'+userId)//уточнить адресу
                .then((res) =>{
                    this.myArticles=res;
                    this.isLoading =false;
                    console.log("myArticles", res);
                },(error)=>{
                    this.isLoading =false;
                    console.error(error);
                    //showErrorMessage(error)
                });
        },
        //додати статтю до Обране
        async addToFavorites(data:ISelectedArticle){
            sendRequest<GenericResponse>('POST','add_favorites_article',data) //уточнить адресу
                .then((res)=>{
                    console.log(res);
                }, (error) =>{
                    console.log(error)
                });
        },
        //отримати перелік наукових сфер
        async getScienceList() {
            let science_:IScience[] = [];
            science_.push({id:1, name:'Фізика', note:''});
            science_.push({id:2, name:'Математика', note:''});
            science_.push({id:3, name:'Хімія', note:''});
            this.sciences=science_;
            /*this.isLoading=true;
            sendRequest<Array<IScience>>('GET', 'science')
                .then((res) =>{
                    this.sciences=res;
                    this.isLoading =false;
                    console.log("sciences", res);
                },(error)=>{
                    this.isLoading =false;
                    console.error(error);
                    //showErrorMessage(error)
                });*/
        },
        //отримати розділи наукових сфер
        async getScienceSectionList() {
            let section:IScientificTheory[]=[];
            section.push({id:1,science_id:1, name:'Кінематика', note:''});
            section.push({id:2,science_id:2, name:'Натуральні числа', note:''});
            section.push({id:3,science_id:3, name:'Неорганічні сполуки', note:''});
            section.push({id:4,science_id:1, name:'Динаміка', note:''});
            section.push({id:5,science_id:2, name:'Дійсні числа', note:''});
            section.push({id:6,science_id:3, name:'Органічні сполуки', note:''});
            section.push({id:7,science_id:1, name:'Статика', note:''});
            section.push({id:8,science_id:2, name:'Іраціональні числа', note:''});
            section.push({id:9,science_id:3, name:'Мінерали', note:''});
            this.scientificSections=section;
            /*this.isLoading=true;
            sendRequest<Array<IScientificTheory>>('GET', 'science_theory')
                .then((res) =>{
                    this.scientificSections=res;
                    this.isLoading =false;
                    console.log("sciences", res);
                },(error)=>{
                    this.isLoading =false;
                    console.error(error);
                    //showErrorMessage(error)
                });*/
        },
    }
});

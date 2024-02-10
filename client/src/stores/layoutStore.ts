import type {IArticle, INewArticleModel, IPeople} from "@/api/type";
import {defineStore} from "pinia";
import MyLocalStorage from "@/services/myLocalStorage";
import type {AuthStoreState} from "@/stores/authStore";


export type LayoutStoreState={
    newArticle: INewArticleModel|null;
    editArticle: IArticle|null;
    savePeople: IPeople|null;
}
export const useLayoutStore = defineStore({
    id: 'layout',
    state: (): LayoutStoreState => ({
        newArticle: null,
        editArticle:null,
        savePeople:null,
    }),
    actions:{
        saveLayoutNewArticle(saveData:INewArticleModel|null){
            console.log('saveData=',saveData)
            this.newArticle = saveData
        },
        restoreLayoutNewArticle():INewArticleModel|null{
            return this.newArticle;
        },
        saveLayoutEditArticle(saveData:IArticle|null){
            console.log('saveData=',saveData)
            this.editArticle = saveData
        },
        restoreLayoutEditArticle():IArticle|null{
            return this.editArticle;
        },
        saveLayoutProfile(savePeople:IPeople|null){
            console.log('savePeople=',savePeople)
            this.savePeople = savePeople;
        },
        restoreLayoutProfile():IPeople|null{
            return this.savePeople;
        }
    }
})
import { defineStore } from 'pinia';
import type { IArticle } from '@/api/type';
import { sendRequest } from '@/api/authApi';

export type ArticleStoreState = {
    articles: IArticle[] | null;
}

export const useArticleStore = defineStore({
    id: 'article',
    state: (): ArticleStoreState => ({
        articles: null,
    }),

    actions: {
        async getArticleList() {
            sendRequest<Array<IArticle>>('GET', 'article')
                .then((res) =>{
                    this.articles=res;
                    //console.log("articles", res);
                },(error)=>{
                    console.error(error);
                    //showErrorMessage(error)
                });
        },
    }
});

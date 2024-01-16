import type {GenericResponse, IEmotion, IReaction} from "@/api/type";
import { defineStore } from 'pinia';
import {sendRequest} from "@/api/authApi";



export type ReactionStoreState = {
   emojiList: IEmotion[];
   isLoading: boolean;
   selectedEmoji:string;
}

export const useReactionStore = defineStore({
   id: 'reaction',
   state: (): ReactionStoreState => ({
      emojiList: [],
      isLoading:false,
      selectedEmoji:''

   }),
   getters:{},

   actions: {
      //отримати список реакцій
      async getEmojiList() {
         this.isLoading=true;
          try {
              const res = await sendRequest<Array<IEmotion>>(
                  'GET',
                  'Emotion/GetAllEmotions'
              );
              this.emojiList = res;
              /*this.emojiList.map((item)=>{
                  item.isSelected=false;
              })*/

              console.log("emojiList = ", res);
          } catch (error) {
              console.error(error);
              // showErrorMessage(error)
          }finally
          {
              this.isLoading = false;
          }
      },
       // отримати обраний користувачем смайл
       async getSelectedEmoji(articleId: string, userId: string) {
           this.isLoading = true;
           try {
               const res = await sendRequest<string>(
                   'GET',
                   'emotion/getSelectedEmotion'
               );
               this.selectedEmoji = res;
               console.log("selectedEmoji = ", res);
           } catch (error) {
               console.error(error);
               // showErrorMessage(error)
           }finally
           {
               this.isLoading = false;
           }
       },

       async saveUserReaction(userReaction: IReaction) {
           try {
               const res = await sendRequest<GenericResponse>(
                   'POST',
                   'reaction/addReaction',
                   undefined,
                   userReaction
               );
               console.log("saveUserReaction =", res);
           } catch (error) {
               console.error(error);
               throw error;

           }finally
           {
               this.isLoading = false;
           }
       },


   }
});
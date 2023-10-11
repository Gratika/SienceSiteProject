import myLog from "@/services/myLog";

export default {

    setItem(name:string, item:any) {
        localStorage.setItem(name, JSON.stringify(item))
        localStorage.setItem(name + '_time', Date.now().toLocaleString())

    },

    getItem(name:string){
        const item = localStorage.getItem(name);
        if(item!=null) {
            myLog("Now " + Date.now())
            myLog("time " + localStorage.getItem(name + '_time'))
            return JSON.parse(item)

        }
        return null
    }
}
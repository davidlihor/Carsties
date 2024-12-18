import {configureStore} from "@reduxjs/toolkit";
import pageSlice from "@/app/redux/pageSlice";

const store = configureStore({
    reducer: {
        page: pageSlice
    }
})

export type State = ReturnType<typeof store.getState>;
export type Dispatch = typeof store.dispatch;
export default store;
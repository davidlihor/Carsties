import {configureStore} from "@reduxjs/toolkit";
import pageSlice from "@/app/redux/pageSlice";
import dataSlice from "@/app/redux/dataSlice";
import bidsSlice from "@/app/redux/bidsSlice";

const store = configureStore({
    reducer: {
        page: pageSlice,
        data: dataSlice,
        bids: bidsSlice
    }
})

export type State = ReturnType<typeof store.getState>;
export type Dispatch = typeof store.dispatch;
export default store;
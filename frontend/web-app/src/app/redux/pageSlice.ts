import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {Page} from "@/app/types";
import {State} from "@/app/redux/store";

const initialState: Page = {
    pageNumber: 1,
    pageSize: 12,
    pageCount: 1,
    searchTerm: "",
    searchValue: "",
    orderBy: "make",
    filterBy: "live",
    seller: "",
    winner: "",
}

const pageSlice = createSlice({
    name: "page",
    initialState,
    reducers: {
        setParams: (state, action: PayloadAction<Partial<Page>>) => {
            if(action.payload.pageNumber){
                return {...state, ...action.payload}
            }else{
                return {...state, ...action.payload, pageNumber: 1}
            }
        },
        setSearch: (state, payload: PayloadAction<string>) => {
            state.searchValue = payload.payload
        },
        reset: () => initialState,
    }
})

export const selectParams = (state: State) => state.page;
export const { setParams, setSearch, reset } = pageSlice.actions;
export default pageSlice.reducer;
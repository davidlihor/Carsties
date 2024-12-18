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
    filterBy: "live"
}

const pageSlice = createSlice({
    name: "page",
    initialState,
    reducers: {
        setParams: (state, payload: PayloadAction<Partial<Page>>) => {
            state.pageNumber = payload.payload.pageNumber || 1
            if(payload.payload.pageSize) state.pageSize = payload.payload.pageSize
            if(payload.payload.pageCount) state.pageCount = payload.payload.pageCount
            if(payload.payload.searchTerm) state.searchTerm = payload.payload.searchTerm
            if(payload.payload.orderBy) state.orderBy = payload.payload.orderBy
            if(payload.payload.filterBy) state.filterBy = payload.payload.filterBy
        },
        setSearch: (state, payload: PayloadAction<string>) => {
            state.searchValue = payload.payload
        },
        reset: (state) => {
            state.pageNumber = 1
            state.pageSize = 12
            state.pageCount = 1
            state.searchTerm = ""
            state.searchValue = ""
            state.orderBy = "make"
            state.filterBy = "live"
        }
    }
})

export const selectParams = (state: State) => state.page
export const { setParams, setSearch, reset } = pageSlice.actions;
export default pageSlice.reducer;
import {Auction, Data, PagedResult} from "@/app/types";
import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {State} from "@/app/redux/store";

const initialState: Data = {
    auctions: [],
    pageCount: 0,
    totalCount: 0,
}

const dataSlice = createSlice({
    name: "data",
    initialState,
    reducers: {
        setData: (state, action: PayloadAction<PagedResult<Auction>>) => {
            state.auctions = action.payload.results;
            state.pageCount = action.payload.pageCount;
            state.totalCount = action.payload.totalCount;
        },
        setCurrentPrice: (state, action: PayloadAction<{auctionId: string, amount: number}>) => {
            state.auctions = state.auctions.map(auction => auction.id === action.payload.auctionId 
                ? {...auction, currentHighBid: action.payload.amount} : auction);
        },
    }
})

export const selectData = (state: State) => state.data;
export const { setData, setCurrentPrice } = dataSlice.actions;
export default dataSlice.reducer;
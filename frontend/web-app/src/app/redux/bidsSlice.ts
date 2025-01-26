import {Bid} from "@/app/types";
import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {State} from "@/app/redux/store";

const bidsSlice = createSlice({
    name: "bid",
    initialState: { bids: [] as Bid[], open: true },
    reducers: {
        setBids: (state, action: PayloadAction<Bid[]>) => {
            state.bids = action.payload
        },
        addBid: (state, action: PayloadAction<Bid>) => {
            state.bids = !state.bids.find(bid => bid.id === action.payload.id) 
                ? [action.payload, ...state.bids] : [...state.bids]
        },
        setOpen: (state, action: PayloadAction<boolean>) => {
            state.open = action.payload
        }
    }
})

export const selectBids = (state: State) => state.bids.bids;
export const selectOpen = (state: State) => state.bids.open;
export const { setBids, addBid, setOpen } = bidsSlice.actions;
export default bidsSlice.reducer;
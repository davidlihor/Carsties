"use client"
import { FieldValues, useForm } from "react-hook-form";
import { Dispatch } from "@/app/redux/store";
import { useDispatch } from "react-redux";
import { postBidForAuction } from "@/app/actions/AuctionActions";
import { addBid } from "@/app/redux/bidsSlice";
import { numberWithCommas } from "@/app/lib/numberFormater";
import toast from "react-hot-toast";

type Props = {
    auctionId: string
    highBid: number
}

export default function BidForm({ auctionId, highBid }: Props) {
    const { register, handleSubmit, reset, formState: { errors } } = useForm();
    const dispatch: Dispatch = useDispatch();

    const onSubmit = async (data: FieldValues) => {
        if (data.amount <= highBid){
            reset();
            return toast.error(`Bid must be at least $${numberWithCommas(highBid + 1)}`)
        }
        postBidForAuction(auctionId, +data.amount)
            .then(response => {
                dispatch(addBid(response));
                reset();
            })
            .catch(error => {
                const res = JSON.parse(error.message);
                console.log(res);
                toast.error(`${res.code} ${res.response || res.status}`);
            })
    }

    return (
        <form onSubmit={handleSubmit(onSubmit)} className="flex items-center border-2 rounded-lg py-2">
            <input
                type="number"
                {...register("amount")}
                className="input-custom"
                placeholder={`Enter your bid (minimum bid is ${numberWithCommas(highBid + 100)})`}
            />
        </form>
    )
}
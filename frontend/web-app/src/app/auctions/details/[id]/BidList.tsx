"use client"
import { Session } from "next-auth";
import { Auction } from "@/app/types";
import { Dispatch } from "@/app/redux/store";
import { useDispatch, useSelector } from "react-redux";
import { selectBids, selectOpen, setBids, setOpen } from "@/app/redux/bidsSlice";
import { useEffect, useState } from "react";
import { getBidsForAuction } from "@/app/actions/AuctionActions";
import toast from "react-hot-toast";
import Heading from "@/app/components/Heading";
import BidItem from "@/app/auctions/details/[id]/BidItem";
import { numberWithCommas } from "@/app/lib/numberFormater";
import EmptyFilter from "@/app/components/EmptyFilter";
import BidForm from "@/app/auctions/details/[id]/BidForm";

type Props = {
    session: Session | null
    auction: Auction
}

export default function BidList({ session, auction }: Props) {
    const [loading, setLoading] = useState(true);
    const bids = useSelector(selectBids);
    const open = useSelector(selectOpen);
    const dispatch: Dispatch = useDispatch();

    const openForBids = new Date(auction.auctionEnd) > new Date();
    const highBid = bids.reduce((previous, current) =>
        previous > current.amount ? previous : current.bidStatus.includes("Accepted") ? current.amount : previous, 0);

    useEffect(() => {
        getBidsForAuction(auction.id)
            .then(result => dispatch(setBids(result)))
            .catch(error => {
                const res = JSON.parse(error.message);
                toast.error(`${res.code} ${res.status}`);
            })
            .finally(() => setLoading(false));
    }, [auction.id, setLoading, dispatch])

    useEffect(() => { dispatch(setOpen(openForBids)) }, [openForBids, dispatch])

    if (loading) return <span>Loading bids...</span>

    return (
        <div className="rounded-lg border-2 border-gray-200">
            <div className="py-2 px-4 bg-white">
                <div className="sticky top-2 bg-white">
                    <Heading title={`Current high bid is ${numberWithCommas(highBid)}`} />
                </div>
            </div>

            <div className="overflow-auto h-[300px] flex flex-col">
                {bids.length > 0 ? (
                    <>{bids.map(bid => (<BidItem key={bid.id} bid={bid} />))}</>
                ) : (
                    <EmptyFilter title="No bids for this item" subTitle="" />
                )}
            </div>

            <div>
                {!open ? (
                    <div className="flex items-center justify-center p-2 text-lg font-semibold">
                        This auction has finished
                    </div>
                ) : !session ? (
                    <div className="flex items-center justify-center p-2 text-lg font-semibold">
                        Login to make a bid
                    </div>
                ) : session && session.user.username !== auction.seller && (
                    <BidForm auctionId={auction.id} highBid={highBid} />
                )}
            </div>
        </div>
    )
}
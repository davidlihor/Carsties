"use client"
import { ReactNode, useCallback, useEffect, useRef } from "react";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { Dispatch } from "@/app/redux/store";
import { useDispatch } from "react-redux";
import { useParams } from "next/navigation";
import { Auction, AuctionFinished, Bid } from "@/app/types";
import { setCurrentPrice } from "@/app/redux/dataSlice";
import { addBid } from "@/app/redux/bidsSlice";
import { Session } from "next-auth";
import toast from "react-hot-toast";
import AuctionCreatedToast from "../components/AuctionCreatedToast";
import { getAuction } from "../actions/AuctionActions";
import AuctionFinishedToast from "../components/AuctionFinishedToast";

type Props = {
    children: ReactNode
    session: Session | null
    notifyUrl: string
}

export default function SignalRProvider({ children, session, notifyUrl }: Props) {
    const connection = useRef<HubConnection | null>(null);
    const dispatch: Dispatch = useDispatch();
    const params = useParams<{ id: string }>();

    const handleAuctionCreated = useCallback((auction: Auction) => {
        if (session?.user.username !== auction.seller) {
            return toast(<AuctionCreatedToast auction={auction} />, { duration: 10000 })
        }
    }, [session?.user.username])

    const handleAuctionFinished = useCallback((finishedAuction: AuctionFinished) => {
        const auction = getAuction(finishedAuction.auctionId);
        return toast.promise(auction, {
            loading: "Loading...",
            success: (auction: Auction) => <AuctionFinishedToast auction={auction} finishedAuction={finishedAuction} />,
            error: () => "Auction finished"
        }, { success: { duration: 10000, icon: null } })
    }, [])

    const handleBidPlaced = useCallback((bid: Bid) => {
        if (bid.bidStatus.includes("Accepted")) {
            dispatch(setCurrentPrice({ auctionId: bid.auctionId, amount: bid.amount }));
        }
        if (params.id === bid.auctionId) {
            dispatch(addBid(bid))
        }
    }, [params.id, dispatch])

    useEffect(() => {
        if (!connection.current) {
            connection.current = new HubConnectionBuilder()
                .withUrl(notifyUrl)
                .withAutomaticReconnect()
                .build();

            connection.current.start()
                .then(() => console.log("Connection started"))
                .catch(err => console.error(err));
        }
        connection.current.on("AuctionCreated", handleAuctionCreated);
        connection.current.on("AuctionFinished", handleAuctionFinished);
        connection.current.on("BidPlaced", handleBidPlaced);

        return () => {
            connection.current?.off("AuctionCreated", handleAuctionCreated);
            connection.current?.off("AuctionCreated", handleAuctionFinished);
            connection.current?.off("BidPlaced", handleBidPlaced);
        }
    }, [handleAuctionCreated, handleAuctionFinished, handleBidPlaced, notifyUrl])

    return (children)
}
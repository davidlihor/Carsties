"use client"
import Countdown, {zeroPad} from "react-countdown";
import { Dispatch } from "../redux/store";
import { useDispatch } from "react-redux";
import { usePathname } from "next/navigation";
import { setOpen } from "../redux/bidsSlice";

type Props = {
    auctionEnd: string
}

const renderer= ({ days, hours, minutes, seconds, completed}:
     {days: number, hours: number, minutes: number, seconds: number, completed: boolean}) => {

    return (
        <div className={`text-white py-1 px-1 rounded-br-lg rounded-tl-lg flex justify-center
        ${completed ? "bg-red-500" : (days === 0 && hours < 10) ? "bg-amber-500" : "bg-green-500"}`}>
            {completed ? (
                <span>Auction Finished</span>
            ) : (
                <span suppressHydrationWarning={true}>
                    {days}:{zeroPad(hours)}:{zeroPad(minutes)}:{zeroPad(seconds)}
                </span>
            )}
        </div>
    )
}

export default function CountdownTimer({auctionEnd}: Props) {
    const dispatch: Dispatch = useDispatch();
    const pathName = usePathname();
    
    const auctionFinished = () => pathName.startsWith("/auctions/details") ? dispatch(setOpen(false)) : null;

    return (
        <div>
            <Countdown date={auctionEnd} renderer={renderer} onComplete={auctionFinished} />
        </div>
    )
}
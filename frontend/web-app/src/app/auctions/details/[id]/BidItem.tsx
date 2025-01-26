import {Bid} from "@/app/types";
import {format} from "date-fns";
import {numberWithCommas} from "@/app/lib/numberFormater";

type Props = {
    bid: Bid
}

export default function BidItem({ bid }: Props) {
    function getBid(){
        let bgColor: string;
        let text: string;
        
        switch (bid.bidStatus){
            case "Accepted":
                bgColor = "bg-green-200";
                text = "Bid Accepted";
                break;
            case "AcceptedBelowReserved":
                bgColor = "bg-amber-500";
                text = "Reserved not met";
                break;
            case "TooLow":
                bgColor = "bg-red-500";
                text = "Bid was too low";
                break;
            default:
                bgColor = "bg-red-200";
                text = "Bid placed after auction finished";
                break;
        }
        return { bgColor, text };
    }
    
    return (
        <div
            className={`border-gray-300 px-3 py-2 flex justify-between items-center mb-2 ${getBid().bgColor}`}>
            <div className="flex flex-col">
                <span>Bidder: {bid.bidder}</span>
                <span className="text-gray-700 text-sm">Time: {format(new Date(bid.bidTime), "dd MMM yyyy h:mm a")}</span>
            </div>
            <div className="flex flex-col text-right">
                <div className="text-xl font-semibold">${numberWithCommas(bid.amount)}</div>
                <div className="flex flex-row items-center">
                    <span>{getBid().text}</span>
                </div>
            </div>
        </div>
    )
}
import CountdownTimer from "@/app/auctions/CountdownTimer";
import CarImage from "@/app/auctions/CarImage";
import {Auction} from "@/app/types";
import {FaLocationDot} from "react-icons/fa6";
import Link from "next/link";

type Props = {
    auction: Auction;
}

export default function AuctionCard({ auction }: Props){
    return (
        <Link href={`/auctions/details/${auction.id}`} className="group flex w-full hover:bg-gray-900/10 rounded-lg border-2">
            <div className="relative w-3/12 min-w-72 bg-gray-200 aspect-[16/10] rounded-lg overflow-hidden">
                <CarImage auction={auction} />
                <div className="absolute bottom-0 right-0">
                    <CountdownTimer auctionEnd={auction.auctionEnd} />
                </div>
            </div>
            <div className="flex flex-col justify-between m-3 ml-7 flex-grow">
                <div>
                    <h3 className="font-bold text-3xl mb-1 text-gray-700">{auction.make} {auction.model}</h3>
                    <p className="text-xl flex items-center gap-1"><FaLocationDot/>Germany, Berlin </p>
                </div>
                <p className="font-semibold text-xl text-end">${auction.reservePrice}</p>
            </div>
        </Link>
    );
}
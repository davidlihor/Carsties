import Link from "next/link"
import { Auction, AuctionFinished } from "../types"
import Image from "next/image"
import { numberWithCommas } from "../lib/numberFormater"

type Props = {
    finishedAuction: AuctionFinished
    auction: Auction
}

export default function AuctionFinishedToast({ auction, finishedAuction }: Props) {
    return (
        <Link href={`/auctions/details/${auction.id}`} className="flex flex-col items-center">
            <div className="flex flex-col items-center gap-2">
                <Image
                    src={`/next.svg`}
                    alt={`Image of ${auction.make} ${auction.model} in ${auction.color}`}
                    height={80}
                    width={80}
                    className="rounded-lg w-auto h-auto"
                />
                <div className="flex flex-col">
                    <span>Acution {auction.make} {auction.model} has finished</span>
                    {finishedAuction.itemSold && finishedAuction.amount ? (
                        <p>Congrats to {finishedAuction.winner} who has won this auction for
                            ${numberWithCommas(finishedAuction.amount)}
                        </p>
                    ) : (
                        <p>This item did not sell</p>
                    )}
                </div>
            </div>
        </Link>
    )
}
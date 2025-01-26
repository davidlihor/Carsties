import Link from "next/link"
import { Auction } from "../types"
import Image from "next/image"

type Props = {
  auction: Auction
}

export default function AuctionCreatedToast({ auction }: Props) {
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
        <span>New Acution! {auction.make} {auction.model}</span>
      </div>
    </Link>
  )
}
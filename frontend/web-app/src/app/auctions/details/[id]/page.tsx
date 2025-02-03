import {getAuction} from "@/app/actions/AuctionActions";
import Heading from "@/app/components/Heading";
import CountdownTimer from "@/app/auctions/CountdownTimer";
import CarImage from "@/app/auctions/CarImage";
import DetailedSpecs from "@/app/auctions/details/[id]/DetailedSpecs";
import {getCurrentSession} from "@/app/actions/AuthActions";
import EditButton from "@/app/auctions/details/[id]/EditButton";
import DeleteButton from "@/app/auctions/details/[id]/DeleteButton";
import BidList from "@/app/auctions/details/[id]/BidList";

export default async function Details({ params } : { params: Promise<{id: string}> }){
    const data = await getAuction((await params).id);
    const session = await getCurrentSession();

    return (
        <div>
            <div className="flex justify-between">
                <div className="flex items-center gap-3">
                    <Heading title={`${data.make} ${data.model}`}/>
                    {session?.user.username === data.seller && (
                        <>
                            <EditButton id={data.id} />
                            <DeleteButton id={data.id} />
                        </>
                    )}
                </div>
                <div className="flex gap-3">
                    <h3 className="text-2xl font-semibold">Time remaining:</h3>
                    <CountdownTimer auctionEnd={data.auctionEnd}/>
                </div>
            </div>

            <div className="grid grid-cols-2 gap-6 mt-3">
                <div className="w-full bg-gray-200 relative aspect-[5/3] rounded-lg overflow-hidden">
                    <CarImage auction={data}/>
                </div>
                <BidList session={session} auction={data} />
            </div>

            <div className="mt-3 grid grid-cols-1 rounded-lg">
                <DetailedSpecs auction={data}/>
            </div>
        </div>
    )
}
import Heading from "@/app/components/Heading";
import AuctionForm from "@/app/auctions/AuctionForm";
import {getAuction} from "@/app/actions/AuctionActions";

export default async function Update({params} : {params: {id: string}}){
    const data = await getAuction(params.id);
    return (
        <div className="mx-auto max-w-[75%] shadow-lg p-10 bg-white rounded-lg">
            <Heading title="Update" />
            <AuctionForm auction={data} />
        </div>
    )
}
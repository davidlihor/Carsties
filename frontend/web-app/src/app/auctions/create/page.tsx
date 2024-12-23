import Heading from "@/app/components/Heading";
import AuctionForm from "@/app/auctions/AuctionForm";

export default function Create() {
    return (
        <div className="mx-auto max-w-[75%] border-2 border-gray-200 p-10 bg-white rounded-lg">
            <Heading title="Sell your car" subtitle="Please enter the details" />
            <AuctionForm />
        </div>
    )
}
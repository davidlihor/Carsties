"use client"
import {Button} from "flowbite-react";
import {useState} from "react";
import {useRouter} from "next/navigation";
import {deleteAuction} from "@/app/actions/AuctionActions";
import toast from "react-hot-toast";

type Props = {
    id: string
}

export default function DeleteButton({id}:Props){
    const [loading, setLoading] = useState(false);
    const router = useRouter();

    async function doDelete(){
        setLoading(true);
        await deleteAuction(id)
            .then(() => router.push(`/`))
            .catch(error => {
                const res = JSON.parse(error.message);
                toast.error(`${res.code} ${res.status}`);
            })
            .finally(() => setLoading(false))
    }

    return (
        <Button color="failure" isProcessing={loading} onClick={doDelete}>
            Delete Auction
        </Button>
    )
}
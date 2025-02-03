"use client"
import {useState} from "react";
import {updateAuctionTest} from "@/app/actions/AuctionActions";
import {Button} from "flowbite-react";

export default function AuthTest() {
    const [loading, setLoading] = useState(false);
    const [result, setResult] = useState<object>();

    function update(){
        setResult(undefined);
        setLoading(true);
        updateAuctionTest()
            .then(res => setResult(res))
            .catch(err => setResult(err))
            .finally(() => setLoading(false));
    }

    return (
        <div className="flex items-center gap-4">
            <Button color={"blue"} isProcessing={loading} onClick={update}>
                Test auth
            </Button>
            <div>
                {JSON.stringify(result, null, 2)}
            </div>
        </div>
    )
}
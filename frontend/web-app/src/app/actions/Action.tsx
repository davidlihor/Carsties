"use server"
import {Auction, Page, PagedResult} from "@/app/types";
import {getServerSession} from "next-auth";
import {authOptions} from "@/auth";

export async function getData(params: Page): Promise<PagedResult<Auction>> {
    const response =
        await fetch(`http://localhost:7002/search?pageSize=${params.pageSize}&pageNumber=${params.pageNumber}&searchTerm=${params.searchTerm}&orderBy=${params.orderBy}&filterBy=${params.filterBy}`);

    if (!response.ok) throw new Error("Failed to fetch data");
    return response.json();
}

export async function updateAuctionTest(){
    const data = {
        mileage: Math.floor(Math.random() * 10000) + 1
    }

    const session = await getServerSession(authOptions);

    const res = await fetch(`http://localhost:7002/auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c`, {
        method: "PUT",
        headers: {
            "Content-type": "application/json",
            "Authorization": `Bearer ${session?.accessToken}`
        },
        body: JSON.stringify(data)
    })

    if(!res.ok) return {status: res.status, message: res.statusText}
    console.log(res)
    return res.statusText
}
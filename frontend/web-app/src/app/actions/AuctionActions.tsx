"use server"
import { Auction, Bid, Page, PagedResult } from "@/app/types";
import { FieldValues } from "react-hook-form";
import agentMethods from "@/app/lib/agent";
import { revalidatePath } from "next/cache";

export async function getData(params: Page) {
    return await agentMethods.get<PagedResult<Auction>>(`/search?pageSize=${params.pageSize}&pageNumber=${params.pageNumber}&searchTerm=${params.searchTerm}&orderBy=${params.orderBy}&filterBy=${params.filterBy}&seller=${params.seller}&winner=${params.winner}`)
}

export async function updateAuctionTest() {
    const data = {
        id: "afbee524-5972-4075-8800-7d1f9d7b0a0c",
        mileage: Math.floor(Math.random() * 10000) + 1
    }
    return await agentMethods.put<object>("auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c", data)
        .catch(error => { throw JSON.stringify(error) });
}

export async function createAuction(data: FieldValues) {
    return await agentMethods.post<Auction>(`auctions`, data)
        .catch(error => { throw JSON.stringify(error) });
}

export async function getAuction(id: string) {
    return await agentMethods.get<Auction>(`auctions/${id}`)
        .catch(error => { throw JSON.stringify(error) });
}

export async function updateAuction(data: FieldValues, id: string) {
    return await agentMethods.put<void>(`auctions/${id}`, data)
        .then(() => revalidatePath(`/auctions/${id}`))
        .catch(error => { throw JSON.stringify(error) });
}

export async function deleteAuction(id: string) {
    return await agentMethods.delete<void>(`auctions/${id}`)
        .catch(error => { throw JSON.stringify(error) });
}

export async function getBidsForAuction(id: string) {
    return await agentMethods.get<Bid[]>(`bids/${id}`)
}

export async function postBidForAuction(auctionId: string, amount: number) {
    return await agentMethods.post<Bid>(`bids?auctionId=${auctionId}&amount=${amount}`, {})
        .catch(error => { throw JSON.stringify(error) });
}
"use server"
import {Auction, Page, PagedResult} from "@/app/types";

export async function getData(params: Page): Promise<PagedResult<Auction>> {
    console.log(params)

    const response =
        await fetch(`http://localhost:7002/search?pageSize=${params.pageSize}&pageNumber=${params.pageNumber}&searchTerm=${params.searchTerm}&orderBy=${params.orderBy}&filterBy=${params.filterBy}`);

    if (!response.ok) throw new Error("Failed to fetch data");
    return response.json();
}
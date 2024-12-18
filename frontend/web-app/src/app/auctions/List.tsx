"use client"
import AuctionCard from "@/app/auctions/AuctionCard";
import AppPagination from "@/app/components/AppPagination";
import {useEffect, useState} from "react";
import {Auction, PagedResult} from "@/app/types";
import {getData} from "@/app/actions/Action";
import Filters from "@/app/auctions/Filters";
import {useDispatch, useSelector} from "react-redux";
import {selectParams, setParams} from "@/app/redux/pageSlice";
import {Dispatch} from "@/app/redux/store";
import EmptyFilter from "@/app/components/EmptyFilter";

export default function List() {
    const [data, setData] = useState<PagedResult<Auction>>();
    const [list, setList] = useState(true);
    const params = useSelector(selectParams);
    const dispatch: Dispatch = useDispatch();

    function setPageNumber(pageNumber: number) {
        dispatch(setParams({pageNumber}));
    }

    useEffect(() => {
        getData(params).then(data => {
            setData(data)
        });
    }, [params])

    if(!data) return <h3>Loading...</h3>

    return (
        <>
            <Filters/>
            {data.totalCount === 0 ? (
                <EmptyFilter showReset />
            ):(
                <>
                    <div className={`grid gap-6 ${list ? "" : "grid-cols-4"}`}>
                        {data.results.map(auction => (
                            <AuctionCard auction={auction} key={auction.id}/>
                        ))}
                    </div>
                    <div className="flex justify-center mt-4">
                        <AppPagination
                            pageChangedAction={setPageNumber}
                            currentPage={params.pageNumber}
                            pageCount={data.pageCount}
                        />
                    </div>
                </>
            )}
        </>
    )
}

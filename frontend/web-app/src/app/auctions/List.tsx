"use client"
import AuctionCard from "@/app/auctions/AuctionCard";
import AppPagination from "@/app/components/AppPagination";
import {useEffect, useState} from "react";
import {getData} from "@/app/actions/AuctionActions";
import Filters from "@/app/auctions/Filters";
import {useDispatch, useSelector} from "react-redux";
import {selectParams, setParams} from "@/app/redux/pageSlice";
import {Dispatch} from "@/app/redux/store";
import EmptyFilter from "@/app/components/EmptyFilter";
import {selectData, setData} from "@/app/redux/dataSlice";

export default function List() {
    const [list, setList] = useState(true);
    const [loading, setLoading] = useState(true);
    const params = useSelector(selectParams);
    const data = useSelector(selectData);
    const dispatch: Dispatch = useDispatch();

    function setPageNumber(pageNumber: number) {
        dispatch(setParams({pageNumber}));
    }

    useEffect(() => {
        getData(params).then(data => {
            dispatch(setData(data));
            setLoading(false);
        });
    }, [params])

    if(loading) return <h3>Loading...</h3>

    return (
        <>
            <Filters/>
            {data.totalCount === 0 ? (
                <EmptyFilter showReset />
            ):(
                <>
                    <div className={`grid gap-6 ${list ? "" : "grid-cols-4"}`}>
                        {data.auctions.map(auction => (
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

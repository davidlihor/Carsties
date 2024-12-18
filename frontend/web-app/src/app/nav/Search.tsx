"use client"
import {FiSearch} from "react-icons/fi";
import {Dispatch, State} from "@/app/redux/store";
import {useDispatch, useSelector} from "react-redux";
import {setParams, setSearch} from "@/app/redux/pageSlice";

export default function Search() {
    const dispatch: Dispatch = useDispatch();
    const searchValue = useSelector((state: State) => state.page.searchValue)

    const search = () => dispatch(setParams({searchTerm: searchValue}))

    return (
        <div className="flex w-[45%] items-center border-2 rounded-full shadow-sm bg-white">
            <button onClick={search}>
                <FiSearch size={35} className="text-black rounded-full cursor-pointer mx-2" />
            </button>
            <input
                type="text"
                placeholder="Type here to Search"
                className="flex-grow bg-transparent focus:outline-none border-transparent focus:border-transparent focus:ring-0 text-gray-600"
                value={searchValue}
                onChange={(e) => dispatch(setSearch(e.target.value))}
                onKeyDown={(e) => { if(e.key === "Enter") search() }}
            />
        </div>
    )
}
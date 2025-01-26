"use client"
import {FiSearch} from "react-icons/fi";
import {Dispatch, State} from "@/app/redux/store";
import {useDispatch, useSelector} from "react-redux";
import {setParams, setSearch} from "@/app/redux/pageSlice";
import {usePathname, useRouter} from "next/navigation";

export default function Search() {
    const router = useRouter();
    const pathName = usePathname();
    const dispatch: Dispatch = useDispatch();
    const searchValue = useSelector((state: State) => state.page.searchValue)

    const search = () => {
        if(pathName !== "/") router.push("/")
        dispatch(setParams({searchTerm: searchValue}));
    }

    return (
        <div className="flex w-[45%] items-center border-2 rounded-full shadow-sm bg-white hover:border-gray-300">
            <button onClick={search}>
                <FiSearch size={35} className="text-black rounded-full cursor-pointer mx-2" />
            </button>
            <input
                type="text"
                placeholder="Type here to Search"
                className="input-custom"
                value={searchValue}
                onChange={(e) => dispatch(setSearch(e.target.value))}
                onKeyDown={(e) => { if(e.key === "Enter") search() }}
            />
        </div>
    )
}
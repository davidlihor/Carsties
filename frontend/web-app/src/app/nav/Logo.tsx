"use client"
import {BiMenu} from "react-icons/bi";
import {useDispatch} from "react-redux";
import {Dispatch} from "@/app/redux/store";
import {reset} from "@/app/redux/pageSlice";
import {useRouter} from "next/navigation";
import {usePathname} from "next/navigation";

export default function Logo() {
    const router = useRouter();
    const path = usePathname()
    const dispatch: Dispatch = useDispatch();

    const doReset = () => {
        if (path !== "/") router.push("/")
        dispatch(reset())
    }

    return (
        <div onClick={doReset}
             className="cursor-pointer flex items-center gap-2 text-4xl font-bold black">
            <BiMenu size={35}/>
            <div>Store</div>
        </div>
    )
}
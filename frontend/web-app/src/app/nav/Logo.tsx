"use client"
import {BiMenu} from "react-icons/bi";
import {useDispatch} from "react-redux";
import {Dispatch} from "@/app/redux/store";
import {reset} from "@/app/redux/pageSlice";

export default function Logo() {
    const dispatch: Dispatch = useDispatch();

    return (
        <div onClick={() => dispatch(reset())}
             className="cursor-pointer flex items-center gap-2 text-4xl font-bold black">
            <BiMenu size={35}/>
            <div>OLX</div>
        </div>
    )
}
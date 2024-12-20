"use client"
import {useDispatch} from "react-redux";
import {Dispatch} from "@/app/redux/store";
import Heading from "@/app/components/Heading";
import {Button} from "flowbite-react";
import {reset} from "@/app/redux/pageSlice";
import {signIn} from "next-auth/react";

type Props = {
    title?: string
    subtitle?: string
    showReset?: boolean
    showLogin?: boolean
    callbackUrl?: string
}

export default function EmptyFilter(
    {title = "No matches", subtitle = "Change filter", showReset, showLogin, callbackUrl}: Props) {
    const dispatch: Dispatch = useDispatch();

    return (
        <div className="h-[40vh] flex flex-col gap-2 justify-center items-center shadow-lg">
            <Heading title={title} subtitle={subtitle} center />
            <div className="mt-4">
                {showReset && (
                    <Button outline onClick={() => dispatch(reset())}>Remove Filters</Button>
                )}
                {showLogin && (
                    <Button outline onClick={() => signIn("auth-server", {callbackUrl})}>Login</Button>
                )}
            </div>
        </div>
    )
}
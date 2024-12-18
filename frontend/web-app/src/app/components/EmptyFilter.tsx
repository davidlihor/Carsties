import {useDispatch} from "react-redux";
import {Dispatch} from "@/app/redux/store";
import Heading from "@/app/components/Heading";
import {Button} from "flowbite-react";
import {reset} from "@/app/redux/pageSlice";

type Props = {
    title?: string
    subtitle?: string
    showReset?: boolean
}

export default function EmptyFilter({title = "No matches", subtitle = "Change filter", showReset}: Props) {
    const dispatch: Dispatch = useDispatch();

    return (
        <div className="h-[40vh] flex flex-col gap-2 justify-center items-center shadow-lg">
            <Heading title={title} subtitle={subtitle} center />
            <div className="mt-4">
                {showReset && (
                    <Button outline onClick={() => dispatch(reset())}>Remove Filters</Button>
                )}
            </div>
        </div>
    )
}
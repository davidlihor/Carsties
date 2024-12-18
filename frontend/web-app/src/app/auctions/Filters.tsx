import {Button, ButtonGroup} from "flowbite-react";
import {useDispatch, useSelector} from "react-redux";
import {Dispatch, State} from "@/app/redux/store";
import {setParams} from "@/app/redux/pageSlice";
import {AiOutlineClockCircle, AiOutlineSortAscending} from "react-icons/ai";
import {BsFillStopCircleFill, BsStopwatchFill} from "react-icons/bs";
import {GiFinishLine, GiFlame} from "react-icons/gi";

const pageSizeButtons = [4, 8, 12];
const orderButtons = [
    {
        label: "Alphabetical",
        icon: AiOutlineSortAscending,
        value: "make",
    },
    {
        label: "End date",
        icon: AiOutlineClockCircle,
        value: "endingSoon",
    },
    {
        label: "Recently added",
        icon: BsFillStopCircleFill,
        value: "new",
    }
]

const filterButtons = [
    {
        label: "Live",
        icon: GiFlame,
        value: "live",
    },
    {
        label: "Ending",
        icon: GiFinishLine,
        value: "endingSoon",
    },
    {
        label: "Completed",
        icon: BsStopwatchFill,
        value: "finished",
    }
]

export default function Filters() {
    const pageSize = useSelector((state: State) => state.page.pageSize);
    const orderBy = useSelector((state: State) => state.page.orderBy);
    const filterBy = useSelector((state: State) => state.page.filterBy);
    const dispatch: Dispatch = useDispatch();

    return (
        <div className="flex justify-between items-center mb-4">
            <div>
                <span className="uppercase text-sm text-gray-500 mr-2">Filter by</span>
                <ButtonGroup>
                    {filterButtons.map(({label, icon: Icon, value}) => (
                        <Button key={value} color={`${filterBy === value ? "blue" : "gray"}`}
                                onClick={() => dispatch(setParams({filterBy: value}))}>
                            <Icon className="mr-3 h-4 w-4"/>
                            {label}
                        </Button>
                    ))}
                </ButtonGroup>
            </div>
            <div>
                <span className="uppercase text-sm text-gray-500 mr-2">Order by</span>
                <ButtonGroup>
                    {orderButtons.map(({label, icon: Icon, value}) => (
                        <Button key={value} color={`${orderBy === value ? "blue" : "gray"}`}
                                onClick={() => dispatch(setParams({orderBy: value}))}>
                            <Icon className="mr-3 h-4 w-4"/>
                            {label}
                        </Button>
                    ))}
                </ButtonGroup>
            </div>
            <div>
                <span className="uppercase text-sm text-gray-500 mr-2">Page size</span>
                <ButtonGroup>
                    {pageSizeButtons.map((value, index) => (
                        <Button onClick={() => dispatch(setParams({pageSize: value}))}
                                key={index} className="focus:ring-0" color={`${pageSize === value ? "blue" : "gray"}`}>
                            {value}
                        </Button>
                    ))}
                </ButtonGroup>
            </div>
        </div>
    )
}
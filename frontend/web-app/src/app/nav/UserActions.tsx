"use client"
import {Dropdown, DropdownDivider, DropdownItem} from "flowbite-react";
import Link from "next/link";
import {User} from "next-auth";
import {HiCog, HiUser} from "react-icons/hi2";
import {AiFillCar, AiFillTrophy, AiOutlineLogout} from "react-icons/ai";
import { signOut } from "next-auth/react";
import {usePathname, useRouter} from "next/navigation";
import {useDispatch} from "react-redux";
import {Dispatch} from "@/app/redux/store";
import {setParams} from "@/app/redux/pageSlice";

type Props = {
    user: Omit<User, "id">
}

export default function UserActions({user}: Props) {
    const router = useRouter();
    const pathName = usePathname();
    const dispatch: Dispatch = useDispatch();

    function setWinner(){
        dispatch(setParams({ winner: user.username, seller: "" }))
        if(pathName !== "/") router.push("/")
    }

    function setSeller(){
        dispatch(setParams({ seller: user.username, winner: "" }))
        if(pathName !== "/") router.push("/")
    }

    return (
        <Dropdown inline label={user.name}>
            <DropdownItem icon={HiUser} onClick={setSeller}>
                Auctions
            </DropdownItem>
            <DropdownItem icon={AiFillTrophy} onClick={setWinner}>
                Auctions Won
            </DropdownItem>
            <DropdownItem icon={AiFillCar}>
                <Link href="/auctions/create">
                    Sell car
                </Link>
            </DropdownItem>
            <DropdownItem icon={HiCog}>
                <Link href="/session">
                    Session (dev only!)
                </Link>
            </DropdownItem>
            <DropdownDivider />
            <DropdownItem icon={AiOutlineLogout} onClick={() => signOut({callbackUrl: "/"})}>
                Sign Out
            </DropdownItem>
        </Dropdown>
    )
}
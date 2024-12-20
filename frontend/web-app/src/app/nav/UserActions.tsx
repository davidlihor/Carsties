"use client"
import {Dropdown, DropdownDivider, DropdownItem} from "flowbite-react";
import Link from "next/link";
import {Session} from "next-auth";
import {HiCog, HiUser} from "react-icons/hi2";
import {AiFillCar, AiFillTrophy, AiOutlineLogout} from "react-icons/ai";
import { signOut } from "next-auth/react";

type Props = {
    session: Session
}

export default function UserActions({session: { user }}: Props) {
    return (
        <Dropdown inline label={user.username}>
            <DropdownItem icon={HiUser}>
                <Link href="/">
                    Auctions
                </Link>
            </DropdownItem>
            <DropdownItem icon={AiFillTrophy}>
                <Link href="/">
                    Auctions Won
                </Link>
            </DropdownItem>
            <DropdownItem icon={AiFillCar}>
                <Link href="/">
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
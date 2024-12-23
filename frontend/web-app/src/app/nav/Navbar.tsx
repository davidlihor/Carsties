import Search from "@/app/nav/Search";
import Logo from "@/app/nav/Logo";
import LoginButton from "./LoginButton";
import { getCurrentSession } from "../actions/AuthActions";
import UserActions from "./UserActions";

export default async function Navbar() {
  const session = await getCurrentSession();
  return (
    <header className="sticky top-0 z-50 flex justify-between bg-white/70 backdrop-blur-sm p-3 items-center text-gray-800 shadow-md">
      <Logo />
      <Search />
      {session ? (<UserActions user={session.user} />) : (<LoginButton />)}
    </header>
  )
}
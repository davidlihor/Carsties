import Search from "@/app/nav/Search";
import Logo from "@/app/nav/Logo";

export default function Navbar() {
  return (
    <header className="sticky top-0 z-50 flex justify-between bg-white/70 backdrop-blur-sm p-3 items-center text-gray-800 shadow-md">
        <Logo />
        <Search />
        <div>Login</div>
    </header>
  )
}
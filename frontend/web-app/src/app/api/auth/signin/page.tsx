import EmptyFilter from "@/app/components/EmptyFilter";

export default async function SignIn({ searchParams }: { searchParams: Promise<{ callbackUrl?: string }> }) {
    return (
        <EmptyFilter
            title="Sign in"
            subTitle={""}
            showLogin
            callbackUrl={(await searchParams).callbackUrl}
        />
    )
}
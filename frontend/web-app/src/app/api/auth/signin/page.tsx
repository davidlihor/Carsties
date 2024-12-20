import EmptyFilter from "@/app/components/EmptyFilter";

export default function SignIn({searchParams}: {searchParams: {callbackUrl: string}}){
    return (
        <EmptyFilter
            title="Sign in"
            subtitle={""}
            showLogin
            callbackUrl={searchParams.callbackUrl}
        />
    )
}
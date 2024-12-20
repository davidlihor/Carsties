import Heading from "../components/Heading";
import { getCurrentSession } from "../actions/AuthActions";
import AuthTest from "@/app/session/AuthTest";

export default async function Page() {
    const session = await getCurrentSession();
    return (
        <div>
            <Heading title="Session" />
            <div className="bg-gray-200 border-2 p-2">
                <h3 className="text-lg">Session</h3>
                <pre className="whitespace-pre-wrap break-all">{JSON.stringify(session, null, 2)}</pre>
            </div>
            <div className="mt-4">
                <AuthTest />
            </div>
        </div>
    )
}
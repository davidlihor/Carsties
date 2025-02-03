import "@/app/globals.css";
import Navbar from "@/app/nav/Navbar";
import React from "react";
import ReduxProvider from "@/app/redux/provider";
import ToasterProvider from "@/app/provider/ToasterProvider";
import SignalRProvider from "@/app/provider/SignalRProvider";
import { getCurrentSession } from "./actions/AuthActions";

export default async function RootLayout({ children }: Readonly<{ children: React.ReactNode; }>) {
  const session = await getCurrentSession();
  const notifyUrl = process.env.NOTIFY_URL;

  return (
    <html lang="en">
      <body>
      <ReduxProvider>
          <ToasterProvider />
          <Navbar />
          <main className="container mx-auto px-5 pt-10">
              <SignalRProvider session={session} notifyUrl={notifyUrl!}>
                  {children}              
              </SignalRProvider>
          </main>
      </ReduxProvider>
      </body>
    </html>
  );
}

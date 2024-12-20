"use client"
import Image from "next/image";
import {useState} from "react";
import {Auction} from "@/app/types";

type Props = {
    auction: Auction
}

export default function CarImage({auction}:Props){
    const [isLoading, setLoading] = useState(true);
    return (
        <Image
            src={`/next.svg`}
            fill
            alt={`Image of ${auction.make} ${auction.model} in ${auction.color}`}
            priority
            className={`object-cover group-hover:opacity-80 duration-500 ease-in-out
            ${isLoading ? "grayscale blur-sm translate-x-full" : "grayscale-0 blur-0 scale-100"}`}
            sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 25vw)"
            onLoad={() => setLoading(false)}
        />
    )
}
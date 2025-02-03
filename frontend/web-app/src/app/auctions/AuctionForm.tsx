"use client"
import {FieldValues, useForm} from "react-hook-form";
import {Button} from "flowbite-react";
import Input from "@/app/components/Input";
import {useEffect} from "react";
import DateInput from "@/app/components/DateInput";
import {createAuction, updateAuction} from "@/app/actions/AuctionActions";
import {usePathname, useRouter} from "next/navigation";
import toast from "react-hot-toast";
import {Auction} from "@/app/types";

type Props = {
    auction?: Auction
}

export default function AuctionForm({auction}: Props) {
    const router = useRouter();
    const pathName = usePathname();
    const {control, handleSubmit, setFocus, reset, formState: {isSubmitting, isValid}} = useForm({
        mode: "onTouched"
    });

    useEffect(() => {
        if(auction){
            const { make, model, color, mileage, year } = auction;
            reset({ make, model, color, mileage, year });
        }
        setFocus("make")
    }, [setFocus, auction, reset])

    async function onSubmit(data: FieldValues){
        let id = "";
        if(pathName === "/auctions/create") {
            await createAuction(data)
                .then((result) => id = result.id)
                .catch(error => {
                    const res = JSON.parse(error.message);
                    toast.error(`${res.code} ${res.status}`);
                })
        }else if(auction) {
            await updateAuction(data, auction.id)
                .then(() => id = auction.id)
                .catch(error => {
                    const res = JSON.parse(error.message);
                    toast.error(`${res.code} ${res.status}`);
                })
        }
        if(id) router.push(`/auctions/details/${id}`)
    }

    return (
        <form className="flex flex-col mt-3" onSubmit={handleSubmit(onSubmit)}>
            <Input label={"Make"} name={"make"} control={control}
                   rules={{required: "Make is required"}}/>
            <Input label={"Model"} name={"model"} control={control}
                   rules={{required: "Model is required"}}/>
            <Input label={"Color"} name={"color"} control={control}
                   rules={{required: "Color is required"}}/>

            <div className="grid grid-cols-2 gap-3">
                <Input label={"Year"} name={"year"} control={control} type="number"
                       rules={{required: "Year is required"}}/>
                <Input label={"Mileage"} name={"mileage"} control={control} type="number"
                       rules={{required: "Mileage is required"}}/>
            </div>

            {pathName === "/auctions/create" && (<>
                <Input label={"Image URL"} name={"imageUrl"} control={control}
                      rules={{required: "Image URL is required"}}/>

                <div className="grid grid-cols-2 gap-3">
                    <Input label={"Reserved Price"} name={"reservePrice"} control={control} type="number"
                           rules={{required: "Reserved Price is required"}}/>
                    <DateInput
                        label={"Auction end date"}
                        name={"auctionEnd"}
                        control={control}
                        dateFormat="dd MM yyyy h:mm a"
                        showTimeSelect
                        rules={{required: "Auction end date is required"}}
                    />
                </div>
            </>)}

            <div className="flex justify-between">
                <Button outline color="gray">Cancel</Button>
                <Button
                    isProcessing={isSubmitting}
                    disabled={!isValid}
                    type="submit"
                    color="blue">Submit</Button>
            </div>
        </form>
    )
}
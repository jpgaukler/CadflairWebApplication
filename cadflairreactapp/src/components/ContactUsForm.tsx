import { Stack, TextInput, Button, Textarea } from "@mantine/core";
import { notifications } from "@mantine/notifications";
import { FormEvent, useState } from "react";


interface ContactUsForm{
    firstName:string,
    lastName:string,
    emailAddress:string,
    companyName:string,
    message:string,
}

export default function ContactUsForm() {
    const [loading, setLoading] = useState<boolean>(false);
    const [form, setForm] = useState<ContactUsForm>({
        firstName:"",
        lastName:"",
        emailAddress:"",
        companyName:"",
        message:"",
    });

    function handleChange(e: FormEvent<HTMLTextAreaElement> | FormEvent<HTMLInputElement>) {
        setForm({
            ...form,
            [e.currentTarget.name]: e.currentTarget.value,
        });
    }

    function handleSubmit() {
        //console.log(JSON.stringify(form));

        setLoading(true);

        fetch("https://cadflairrestapi.azurewebsites.net/api/v1/contact-us", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(form),
        })
            .then(res => {
                console.log(res);

                if (res.ok) {
                    notifications.show({
                        withCloseButton: true,
                        autoClose: 5000,
                        color: "green",
                        message: 'Message Sent!',
                    });
                }
                else {
                    notifications.show({
                        withCloseButton: true,
                        autoClose: 5000,
                        color: "red",
                        message: "Failed to send message!",
                    });
                }

                // reset button state
                setLoading(false);
            })
            .catch(error => {
                console.error(error);
            });
    }

    return (
        <>
            <Stack w="100%" align="center">
                <TextInput
                    name="firstName"
                    onChange={handleChange}
                    size="md"
                    radius="sm"
                    label="First Name"
                    placeholder="First Name"
                    required
                    w="100%"
                />
                <TextInput
                    name="lastName"
                    onChange={handleChange}
                    size="md"
                    radius="sm"
                    label="Last Name"
                    placeholder="Last Name"
                    required
                    w="100%"
                />
                <TextInput
                    name="emailAddress"
                    onChange={handleChange}
                    size="md"
                    radius="sm"
                    label="Email Address"
                    placeholder="Email Address"
                    required
                    w="100%"
                />
                <TextInput
                    name="companyName"
                    onChange={handleChange}
                    size="md"
                    radius="sm"
                    label="Company Name"
                    placeholder="Company Name"
                    w="100%"
                />
                <Textarea
                    name="message"
                    onChange={handleChange}
                    size="md"
                    radius="sm"
                    label="Message"
                    placeholder="Message"
                    autosize
                    minRows={5}
                    w="100%"
                />
                <Button onClick={handleSubmit} loading={loading}>Submit</Button>
            </Stack>
        </>
    )
}
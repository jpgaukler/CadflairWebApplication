import { Stack, TextInput, Button, Textarea } from "@mantine/core";



export default function ContactUsForm() {
    return (
        <>
            <Stack w="100%" align="center">
                <TextInput
                    size="md"
                    radius="sm"
                    label="First Name"
                    placeholder="First Name"
                    withAsterisk
                    w="100%"
                />
                <TextInput
                    size="md"
                    radius="sm"
                    label="Last Name"
                    placeholder="Last Name"
                    withAsterisk
                    w="100%"
                />
                <TextInput
                    size="md"
                    radius="sm"
                    label="Email Address"
                    placeholder="Email Address"
                    withAsterisk
                    w="100%"
                />
                <TextInput
                    size="md"
                    radius="sm"
                    label="Company Name"
                    placeholder="Company Name"
                    w="100%"
                />
                <Textarea
                    size="md"
                    radius="sm"
                    label="Message"
                    placeholder="Message"
                    autosize
                    minRows={5}
                    w="100%"
                />
                <Button>Submit</Button>
            </Stack>
        </>
    )
}
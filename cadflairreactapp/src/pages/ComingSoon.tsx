import { Text, Anchor, Container, Grid, Stack, Title, Group } from "@mantine/core";
import { IconHourglass, IconMail, IconMessages } from "@tabler/icons-react";
import ContactUsForm from "../components/ContactUsForm";



export default function ComingSoon() {
    return (
        <>
            <Container size="lg" py="50px">
                <Grid>
                    <Grid.Col span={12} py="50px">
                        <Group align="center">
                            <IconHourglass />
                            <Title order={1}>Great things coming soon!</Title>
                        </Group>
                        <Text>We are a small development team with big ideas. This page is still in progress, but come back soon to see more!</Text>
                    </Grid.Col>
                    <Grid.Col span={{ sm: 12, lg: 6 }} p="lg">
                        <Stack>
                            <Group align="center">
                                <IconMail size={30} />
                                <Title order={3}>Stay Informed</Title>
                            </Group>
                            <Text>Want to be the first to know about Cadflair news? Join our mailing list to stay informed about Cadflair development.</Text>
                            <ContactUsForm />
                        </Stack>
                    </Grid.Col>
                    <Grid.Col span={{ sm: 12, lg: 6 }} p="lg">
                        <Stack>
                            <Group align="center">
                                <IconMessages size={30} />
                                <Title order={3}>How can Cadflair help your business?</Title>
                            </Group>
                            <Text>Take your sales process to the next level. Click to learn more!</Text>
                            <Anchor href="/">Learn More</Anchor>
                        </Stack>
                    </Grid.Col>
                </Grid>
            </Container>
        </>
    )
}
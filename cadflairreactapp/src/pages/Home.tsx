import { Container, Title, Text, List, Group, Button, Image, ThemeIcon, rem, Stack, Paper, TextInput, Textarea, Box, Anchor, Grid } from "@mantine/core";
import { IconArrowBarUp, IconArrowDown, IconArrowRight, IconCheck, IconChevronDown, IconCircleCheck, IconEdit, IconShare } from "@tabler/icons-react";
import classes from './Home.module.css';
import logo from '../assets/cadflair_logo.svg';
import heroImg from '../assets/product_mockup.png';
import ContactUsForm from "../components/ContactUsForm";

export default function Home() {

    return (
        <>
            <Container size="lg">
                <Group wrap="nowrap" justify="space-between" py="100px">
                    <div>
                        <Title order={1} size="3rem" fw="300">
                            <span className={`${classes.titleFade} ${classes.delay1}`}>3D Catalog</span><br />
                            <span className={`${classes.titleFade} ${classes.delay2}`}>Made</span><br />
                            <span className={`${classes.titleFade} ${classes.delay3}`}><strong>Easy</strong></span><br />
                        </Title>
                        <svg className={`${classes.vectorUnderline} ${classes.delay4}`} viewBox="0 0 206 26" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M3 23C89.445 5.91642 175.379 -4.33371 203 9.33313" strokeWidth="6" strokeLinecap="round" />
                        </svg>
                        <div className={`${classes.titleFade} ${classes.delay5}`}>
                            <Text my="md">Revolutionize your CAD library today.</Text>
                            <a href="demo/categories">
                                <Button rightSection={<IconArrowRight size={20} stroke={2} />}>Try Demo</Button>
                            </a>
                        </div>
                    </div>
                    <Image src={heroImg} alt="hero image" maw={700} visibleFrom="sm"/>
                </Group>
                <Anchor href="#learn-more" c="black">
                    <Stack w="100%" align="center" gap="xs" py="lg">
                        <Text size="lg">Learn More</Text>
                        <IconChevronDown size={24} stroke={1} />
                    </Stack>
                </Anchor>
            </Container>

            <div className={classes.content}>
                <Container size="lg" id="learn-more">
                    <Grid>
                        {/*What is cadflair?*/}
                        <Grid.Col span={{ sm: 12, md: 6 }} pt="100px" px="xl">
                            <Stack>
                                <Group align="center">
                                    <Title order={2} size="2.5rem" fw="600">What is</Title>
                                    <Image src={logo} alt="Cadflair Logo" h={40} />
                                    <Title order={2} size="2.5rem" fw="600">?</Title>
                                </Group>
                                <Text>
                                    Cadflair is an digital product catalog for manufacturing companies. Our mission is to make it simple for companies to share CAD data on the web.
                                    Don't have CAD data available? No problem. At Cadflair, we will work with your team to build a digital library for your products, so you can spend more time growing your business.<br />
                                    <br />
                                    <strong>Customization for Any Industry</strong><br />
                                    No two manufacturing processes are alike. That's why we specialize in creating CAD models that are as unique as your business.
                                    From automotive components to intricate machinery, our team generates models that align with your industry-specific requirements.<br />
                                    <br />
                                    <strong>Efficiency Redefined</strong><br />
                                    Time is money, and we understand urgency in manufacturing. Our CAD models are not just aesthetically pleasing, they're optimized for efficiency.
                                    This means that all models can be viewed in a web-browser, on any device, anytime, anywhere.<br />
                                    <br />
                                    <strong>Collaborative Creation</strong><br />
                                    Your vision, our expertise. We collaborate closely with your team to understand your products and expectations.
                                    Our iterative process ensures that each CAD model is a result of collaborative refinement, guaranteeing a final product that aligns with your expectations.
                                </Text>
                            </Stack>
                        </Grid.Col>

                        {/*Why Cadflair?*/}
                        <Grid.Col span={{ sm: 12, md: 6 }} pt="100px" px="xl">
                            <Stack>
                                <Group align="center">
                                    <Title order={2} size="2.5rem" fw="600">Why</Title>
                                    <Image src={logo} alt="Cadflair Logo" h={40} />
                                    <Title order={2} size="2.5rem" fw="600">?</Title>
                                </Group>
                                <Text>
                                    In today's connected world, CAD data is not just for manufacturing, it is a vital tool for communicating product details to customers.
                                    By providing direct access to your CAD data, you can empower your customers to explore the details of every design,
                                    fostering a deeper understanding of your products, and building confidence in your brand.
                                </Text>
                                <List
                                    spacing="lg"
                                    icon={
                                        <ThemeIcon size={36} radius="xl" color="dark">
                                            <IconCircleCheck />
                                        </ThemeIcon>
                                    }
                                >
                                    <List.Item>
                                        View 3D CAD models on any device
                                    </List.Item>
                                    <List.Item>
                                        Share models with a click
                                    </List.Item>
                                    <List.Item>
                                        Get notified when customers download your models
                                    </List.Item>
                                    <List.Item>
                                        Improve lead generation and conversion
                                    </List.Item>
                                    <List.Item>
                                        Increase buyer confidence
                                    </List.Item>
                                </List>
                            </Stack>
                        </Grid.Col>

                        {/*How it works?*/}
                        <Grid.Col span={12} pt="100px">
                            <Title order={2} mb="md">How It Works</Title>
                        </Grid.Col>
                        <Grid.Col span={{ sm: 12, md: 4 }}>
                            <Paper withBorder mih="430px" p="md">
                                <Stack h="100%">
                                    <Text fw="600" ta="center" style={{ fontSize: rem(100) }}>1</Text>
                                    <Group align="center">
                                        <IconEdit size={24} stroke={1} />
                                        <Text size="xl" fw="600">Create</Text>
                                    </Group>
                                    <Text>
                                        Meet with our team to discuss your products, identify design parameters, and aggregate model dimensions.
                                        We will create CAD files for your digital catalog, and review them with your team to ensure the details are just right.
                                    </Text>
                                </Stack>
                            </Paper>
                        </Grid.Col>
                        <Grid.Col span={{ sm: 12, md: 4 }}>
                            <Paper withBorder mih="430px" p="md">
                                <Stack h="100%">
                                    <Text fw="600" ta="center" style={{ fontSize: rem(100) }}>2</Text>
                                    <Group align="center">
                                        <IconArrowBarUp size={24} stroke={1} />
                                        <Text size="xl" fw="600">Publish</Text>
                                    </Group>
                                    <Text>
                                        We will upload your models to Cadflair, and generate a filterable catalog for your product library, along with a web page for each unique model.
                                        Models are stored on the latest cloud infrastructure to ensure availability and reliability.
                                    </Text>
                                </Stack>
                            </Paper>
                        </Grid.Col>
                        <Grid.Col span={{ sm: 12, md: 4 }}>
                            <Paper withBorder p="md" mih="430px">
                                <Stack h="100%">
                                    <Text fw="600" ta="center" style={{ fontSize: rem(100) }}>3</Text>
                                    <Group align="center">
                                        <IconShare size={24} stroke={1} />
                                        <Text size="xl" fw="600">Share</Text>
                                    </Group>
                                    <Text>
                                        Provide your customers with CAD data on demand. Share your catalog on social media, or embed it directly into your company's site.
                                        View models on any device, without installing CAD packages or plugins.
                                    </Text>
                                    <Group justify="flex-end" w="100%">
                                        <a href="demo/categories">
                                            <Button rightSection={<IconArrowRight size={20} stroke={2} />}>Try Demo</Button>
                                        </a>
                                    </Group>
                                </Stack>
                            </Paper>
                        </Grid.Col>

                        {/*Contact us*/}
                        <Grid.Col span={12} py="100px">
                            <Paper shadow="xl" p="md" id="contact-us">
                                <Stack align="center">
                                    {/*<MudIcon Icon="@Icons.Material.Filled.Email" Size="Size.Large" />*/}
                                    <Title order={4}><strong>Contact Us</strong></Title>
                                    <Text>
                                        Cadflair is currently in development, but we are looking for early adopters to help us build a rock solid product.
                                        Want to be involved? Please reach out if you think Cadflair could help your business!
                                    </Text>

                                    <ContactUsForm />
                                </Stack>
                            </Paper>
                        </Grid.Col>
                    </Grid>
                </Container>
            </div>
        </>
    );
}




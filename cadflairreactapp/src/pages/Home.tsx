import { Container, Title, Text, List, Group, Button, Image, ThemeIcon, rem, Stack, Paper } from "@mantine/core";
import classes from './Home.module.css';
import logo from '../assets/cadflair_logo.svg';
import heroImg from '../assets/product_mockup.png';

export default function Home() {

    return (
        <>
            <Container size="xl">
                <Group wrap="nowrap" justify="space-between" my="xl">
                    <div>
                        <Title order={1} style={{ fontSize: "4rem" }}>
                            <span className={`${classes.titleFade} ${classes.delay1}`}>3D Catalog</span><br />
                            <span className={`${classes.titleFade} ${classes.delay2}`}>Made</span><br />
                            <span className={`${classes.titleFade} ${classes.delay3}`}><strong>Easy</strong></span><br />
                        </Title>
                        <svg className={`${classes.vectorUnderline} ${classes.delay4}`} viewBox="0 0 206 26" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M3 23C89.445 5.91642 175.379 -4.33371 203 9.33313" strokeWidth="6" strokeLinecap="round" />
                        </svg>
                        <div className={`${classes.titleFade} ${classes.delay5}`}>
                            <Text>Revolutionize your CAD library today.</Text>
                            {/*catalog demo*/}
                            <a href="demo/categories">
                                <Button>Try Demo</Button>
                            </a>
                        </div>
                    </div>
                    <Image src={heroImg} alt="hero image" maw={800} />
                </Group>
                <Stack w="100%" align="center">
                    <Text>Learn More</Text>
                    <Text>down arrow</Text>
                </Stack>
            </Container>

            <div className={ classes.content }>
                <Container size="md">
                    <Stack align="center">
                        <Text>What is</Text>
                        <Image src={logo} alt="Cadflair Logo" h={10} />
                        <Text>?</Text>
                    </Stack>
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

                    <Stack align="center">
                        <Text>Why</Text>
                        <Image src={logo} alt="Cadflair Logo" h="40" />
                        <Text>?</Text>
                    </Stack>
                    <Text>
                        In today's connected world, CAD data is not just for manufacturing, it is a vital tool for communicating product details to customers.
                        By providing direct access to your CAD data, you can empower your customers to explore the details of every design,
                        fostering a deeper understanding of your products, and building confidence in your brand.
                    </Text>
                    <List>
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

                    <Text>How It Works</Text>
                    <Paper withBorder mih="415px">
                        <Stack h="100%">
                            <Text><strong>1</strong></Text>
                            <Stack align="center">
                                {/*<IconCheck style={{ width: rem(12), height: rem(12) }} stroke={1.5} />*/}
                                <Text>Create</Text>
                            </Stack>
                            <Text>
                                Meet with our team to discuss your products, identify design parameters, and aggregate model dimensions.
                                We will create CAD files for your digital catalog, and review them with your team to ensure the details are just right.
                            </Text>
                        </Stack>
                    </Paper>
                    <Paper withBorder mih="415px">
                        <Stack h="100%">
                            <Text><strong>2</strong></Text>
                            <Stack align="center">
                                {/*<MudIcon Icon="@Icons.Material.Filled.Upload" />*/}
                                <Title order={6}>Publish</Title>
                            </Stack>
                            <Text>
                                We will upload your models to Cadflair, and generate a filterable catalog for your product library, along with a web page for each unique model.
                                Models are stored on the latest cloud infrastructure to ensure availability and reliability.
                            </Text>
                        </Stack>
                    </Paper>
                    <Paper withBorder mih="415px">
                        <Stack h="100%">
                            <Text><strong>3</strong></Text>
                            <Stack align="center">
                                {/*<MudIcon Icon="@Icons.Material.Filled.Share" />*/}
                                <Title order={6}>Share</Title>
                            </Stack>
                            <Text>
                                Provide your customers with CAD data on demand. Share your catalog on social media, or embed it directly into your company's site.
                                View models on any device, without installing CAD packages or plugins.
                            </Text>
                            <Stack justify="flex-end">
                                <a href="/demo/categories" >
                                    <Button>Try Demo</Button>
                                </a>
                            </Stack>
                        </Stack>
                    </Paper>

                    <Paper>
                        <Stack align="center">
                            {/*<MudIcon Icon="@Icons.Material.Filled.Email" Size="Size.Large" />*/}
                            <Title order={4}><strong>Contact Us</strong></Title>
                        </Stack>
                        <Text>
                            Cadflair is currently in development, but we are looking for early adopters to help us build a rock solid product.
                            Want to be involved? Please reach out if you think Cadflair could help your business!
                        </Text>
                        {/*<ContactUsForm />*/}
                    </Paper>
                </Container>
            </div>
        </>
    );
}




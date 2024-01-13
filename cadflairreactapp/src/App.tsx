import '@mantine/core/styles.css';
import '@mantine/notifications/styles.css';
import { MantineProvider, createTheme} from '@mantine/core';
import { Notifications } from '@mantine/notifications';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Layout from './components/Layout';
import Home from './pages/Home';
import Categories from './pages/Categories';
import NoPage from './pages/NoPage';
import Products from './pages/Products';
import TestHooks from './pages/TestHooks';
import ProductDetails from './pages/ProductDetails';
import ComingSoon from './pages/ComingSoon';

const theme = createTheme({
    primaryColor: "cadflairBlue",
    primaryShade: 3,
    colors: {
        // Add your color
        cadflairBlue: [
            "#e1faff",
            "#cbefff",
            "#9adbff",
            "#50C0FF",
            "#3bb6fe",
            "#21acfe",
            "#09a7ff",
            "#0091e4",
            "#0081cd",
            "#0070b6"
        ],
    },
});


export default function App() {

    return (
        <>
            <MantineProvider theme={theme}>
                <Notifications />
                <BrowserRouter>
                    <Routes>
                        <Route path="/" element={<Layout />}>
                            <Route index element={<Home />} />
                            <Route path="comingsoon" element={<ComingSoon />} />
                            <Route path=":companyName/categories" element={<Categories />} />
                            <Route path=":companyName/categories/:categoryName" element={<Categories />} />
                            <Route path=":companyName/products/:productDefinitionName" element={<Products />} />
                            <Route path=":companyName/products/:productDefinitionName/:partNumber" element={<ProductDetails />} />
                            <Route path=":companyName/test" element={<TestHooks />} />
                            <Route path="*" element={<NoPage />} />
                        </Route>
                    </Routes>
                </BrowserRouter>
            </MantineProvider>
        </>
    );
}

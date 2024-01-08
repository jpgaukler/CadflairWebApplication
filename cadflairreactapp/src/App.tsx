import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Layout from './components/Layout';
import Home from './pages/Home';
import Categories from './pages/Categories';
import NoPage from './pages/NoPage';
import Products from './pages/Products';
import TestHooks from './pages/TestHooks';
import ProductDetails from './pages/ProductDetails';

export default function App() {

    return (
        <>
            <BrowserRouter>
                <Routes>
                    <Route path="/" element={<Layout />}>
                        <Route index element={<Home />} />
                        <Route path=":companyName/categories" element={<Categories />} />
                        <Route path=":companyName/categories/:categoryName" element={<Categories />} />
                        <Route path=":companyName/products/:productDefinitionName" element={<Products />} />
                        <Route path=":companyName/products/:productDefinitionName/:partNumber" element={<ProductDetails />} />
                        <Route path=":companyName/test" element={<TestHooks />} />
                        <Route path="*" element={<NoPage />} />
                    </Route>
                </Routes>
            </BrowserRouter>
        </>
    );
}

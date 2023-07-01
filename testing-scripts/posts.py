from selenium import webdriver
from selenium.webdriver.common.keys import Keys
from dotenv import load_dotenv
import os
import time
from randomText import RandomTextClient
import threading
from queue import Queue

# Load environment variables from .env file
load_dotenv()

# Get the credentials
username = os.getenv("APPUSERNAME")
password = os.getenv("PASSWORD")

client = RandomTextClient()

baseUrl = "https://localhost:7272"
baseUrlProd = "https://tosic-blog.azurewebsites.net"
post_ids = Queue()


def login(driver):
    # open the website and login
    driver.get(baseUrl + "/Identity/Account/Login")
    time.sleep(5)
    username_field = driver.find_element(by="id", value="username")
    password_field = driver.find_element(by="id", value="password")
    username_field.send_keys(username)
    password_field.send_keys(password)
    password_field.send_keys(Keys.RETURN)
    time.sleep(1)


def create_blog_post(driver, i):
    driver.get(baseUrl + "/BlogPosts/Create")
    time.sleep(1)
    if i % 2 == 0:
        title = "Dad joke"
        df = client.dad.get_dad_joke()
        joke = df.iloc[0, 1]
    else:
        title = "Geek joke"
        df = client.geek.get_joke()
        joke = df.iloc[0, 0]
    title_field = driver.find_element(by="id", value="post-title")
    content_field = driver.find_element(by="id", value="post-content")
    title_field.send_keys(title)
    content_field.send_keys(joke)
    content_field.send_keys(Keys.RETURN)
    time.sleep(1)


def create_comment(driver, postId, thread_id):
    print(f"Thread: {thread_id} working on post with id: {postId}")
    driver.get(baseUrl + "/BlogPosts/Details/" + str(postId))
    time.sleep(1)
    add_comment = driver.find_element(by="id", value="add-comment")
    add_comment.click()
    time.sleep(1)
    title_field = driver.find_element(by="id", value="title-input")
    content_field = driver.find_element(by="id", value="content-input")
    submit_btn = driver.find_element(by="id", value="submit-btn")
    title = "Random Text"
    df = client.meta.get_doc(paragraph_size=4, sentence_size=32)
    content = df.iloc[0, 0]
    title_field.send_keys(title)
    content_field.send_keys(content)
    submit_btn.click()
    time.sleep(1)



def get_latest_post_id(driver):
    driver.get(baseUrl)
    time.sleep(1)
    post = driver.find_element(by="id", value="post")
    post.click()
    time.sleep(1)
    current_url = driver.current_url
    latest_post_id = int(current_url.split("/")[-1])
    return latest_post_id


def worker(thread_id, postIds):
    # initialise the driver
    driver = webdriver.Firefox()
    login(driver)
    # Creating blog posts
    for i in range(6):
        create_blog_post(driver, i)
    driver.get(baseUrl)
    time.sleep(1)
    # Create comments for each postId
    for postId in postIds:
        create_comment(driver, postId, thread_id)
    # close the driver
    driver.quit()


# initialise the driver
driver = webdriver.Firefox()
login(driver)
latest_post_id = get_latest_post_id(driver)
print(latest_post_id)
driver.quit()

# Generate all post ids from latest_post_id to 5
all_post_ids = list(range(latest_post_id, 4, -1))

num_threads = 5

# Split the post ids into num_threads parts
post_ids_split = [all_post_ids[i::num_threads] for i in range(num_threads)]

# Create and start threads
threads = []
for i in range(num_threads):
    t = threading.Thread(target=worker, args=(i, post_ids_split[i]))
    threads.append(t)
    t.start()

# Wait for all threads to complete
for t in threads:
    t.join()

print("All tasks completed.")
